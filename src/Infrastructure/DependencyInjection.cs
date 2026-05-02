using System.Text;
using Application.Abstractions.Authentication;
using Application.Abstractions.Data;
using Application.Abstractions.Users;
using Infrastructure.Authentication;
using Infrastructure.DomainEvents;
using Infrastructure.Outbox;
using Infrastructure.Persistence;
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
        services.Configure<RefreshTokenCleanupOptions>(configuration.GetSection("Auth:RefreshTokenCleanup"));
        services.Configure<OutboxOptions>(configuration.GetSection("Outbox"));
        services.AddScoped<IDomainEventsDispatcher, DomainEventsDispatcher>();

        services.AddSingleton<IPasswordHasher, PasswordHasher>();
        services.AddSingleton<ITokenProvider, TokenProvider>();
        services.AddScoped<IRefreshTokenRepository, RefreshTokenRepository>();
        services.AddScoped<IUnitOfWork, UnitOfWork>();
        services.AddScoped<IUserContext, UserContext>();
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddHostedService<RefreshTokenCleanupService>();
        services.AddHostedService<OutboxProcessorService>();

        return services;
    }
}
