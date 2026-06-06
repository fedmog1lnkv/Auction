using System.Text;
using Application.Abstractions.Authentication;
using Application.Abstractions.Bids;
using Application.Abstractions.Data;
using Application.Abstractions.LotPhotos;
using Application.Abstractions.Lots;
using Application.Abstractions.Storage;
using Application.Abstractions.Users;
using Infrastructure.Authentication;
using Infrastructure.Bids;
using Infrastructure.IntegrationEvents;
using Infrastructure.LotPhotos;
using Infrastructure.Lots;
using Infrastructure.Outbox;
using Infrastructure.Persistence;
using Infrastructure.Storage;
using Infrastructure.Users;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;

namespace Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("Database")
                               ?? throw new InvalidOperationException("ConnectionStrings:Database is missing");
        var jwtOptions = configuration.GetSection("Jwt").Get<JwtOptions>()
                         ?? throw new InvalidOperationException("Jwt section is missing");

        if (string.IsNullOrWhiteSpace(jwtOptions.Secret) || Encoding.UTF8.GetByteCount(jwtOptions.Secret) < 32)
            throw new InvalidOperationException("Jwt:Secret must be at least 32 bytes for HS256");

        if (string.IsNullOrWhiteSpace(jwtOptions.Issuer))
            throw new InvalidOperationException("Jwt:Issuer is missing");

        if (string.IsNullOrWhiteSpace(jwtOptions.Audience))
            throw new InvalidOperationException("Jwt:Audience is missing");

        var s3Options = BuildS3Options(configuration);
        var maxLotPhotos = configuration.GetValue<int?>("S3:MaxLotPhotos") ?? 10;
        var maxPhotoSizeBytes = configuration.GetValue<long?>("S3:MaxPhotoSizeBytes") ?? 5 * 1024 * 1024;
        var presignedUrlTtlSeconds = configuration.GetValue<int?>("S3:PresignedUrlTtlSeconds") ?? 900;

        if (maxLotPhotos <= 0)
            throw new InvalidOperationException("S3:MaxLotPhotos must be greater than 0");

        if (maxPhotoSizeBytes <= 0)
            throw new InvalidOperationException("S3:MaxPhotoSizeBytes must be greater than 0");

        if (presignedUrlTtlSeconds <= 0)
            throw new InvalidOperationException("S3:PresignedUrlTtlSeconds must be greater than 0");

        services.AddDbContext<ApplicationDbContext>(options =>
            options
                .UseNpgsql(connectionString)
                .UseSnakeCaseNamingConvention());
        services.Configure<JwtOptions>(configuration.GetSection("Jwt"));

        services
            .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateIssuerSigningKey = true,
                    ValidateLifetime = true,
                    ValidIssuer = jwtOptions.Issuer,
                    ValidAudience = jwtOptions.Audience,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtOptions.Secret))
                };
            });

        services.AddAuthorization();
        services.AddHttpContextAccessor();
        services.AddSignalR();
        services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(LotStartedDomainEventHandler).Assembly));
        services.Configure<RefreshTokenCleanupOptions>(configuration.GetSection("Auth:RefreshTokenCleanup"));
        services.Configure<OutboxOptions>(configuration.GetSection("Outbox"));

        services.AddSingleton<IPasswordHasher, PasswordHasher>();
        services.AddSingleton<ITokenProvider, TokenProvider>();
        services.AddSingleton<IObjectStorage>(_ => new S3ObjectStorage(s3Options));
        services.AddSingleton<IImageVariantGenerator, ImageSharpVariantGenerator>();
        services.AddSingleton<ILotPhotoPolicy>(_ =>
            new LotPhotoPolicy(maxLotPhotos, maxPhotoSizeBytes, presignedUrlTtlSeconds));
        services.AddScoped<IRefreshTokenRepository, RefreshTokenRepository>();
        services.AddScoped<IUnitOfWork, UnitOfWork>();
        services.AddScoped<IBidRepository, BidRepository>();
        services.AddScoped<ILotRepository, LotRepository>();
        services.AddScoped<ILotPhotoRepository, LotPhotoRepository>();
        services.AddScoped<IUserContext, UserContext>();
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddHostedService<RefreshTokenCleanupService>();
        services.AddHostedService<OutboxProcessorService>();

        return services;
    }

    private static S3Options BuildS3Options(IConfiguration configuration)
    {
        var options = configuration.GetSection("S3").Get<S3Options>() ?? new S3Options();

        if (string.IsNullOrWhiteSpace(options.Endpoint))
            throw new InvalidOperationException("S3:Endpoint is missing");

        if (string.IsNullOrWhiteSpace(options.Bucket))
            throw new InvalidOperationException("S3:Bucket is missing");

        if (string.IsNullOrWhiteSpace(options.AccessKey))
            throw new InvalidOperationException("S3:AccessKey is missing");

        if (string.IsNullOrWhiteSpace(options.SecretKey))
            throw new InvalidOperationException("S3:SecretKey is missing");

        if (string.IsNullOrWhiteSpace(options.PublicBaseUrl))
            throw new InvalidOperationException("S3:PublicBaseUrl is missing");

        return options;
    }
}