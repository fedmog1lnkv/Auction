using System.Security.Cryptography;
using Application.Abstractions.Authentication;
using Application.Abstractions.Data;
using Application.Abstractions.Users;
using Domain.Users;
using MediatR;
using SharedKernel;

namespace Application.Auth.Login;

public sealed class LoginQueryHandler(
    IUserRepository userRepository,
    ITokenProvider tokenProvider,
    IPasswordHasher passwordHasher,
    IRefreshTokenRepository refreshTokenRepository,
    IUnitOfWork unitOfWork) : IRequestHandler<LoginQuery, Result<LoginResponse>>
{
    public async Task<Result<LoginResponse>> Handle(LoginQuery query, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(query.Email) || string.IsNullOrWhiteSpace(query.Password))
            return Result.Failure<LoginResponse>(UserErrors.Unauthorized());

        var user = await userRepository.GetByEmailAsync(query.Email, cancellationToken);
        if (user is null || !passwordHasher.Verify(query.Password, user.PasswordHash))
            return Result.Failure<LoginResponse>(UserErrors.Unauthorized());

        var accessToken = tokenProvider.Create(user);
        var refreshTokenValue = Convert.ToBase64String(RandomNumberGenerator.GetBytes(64));

        var refreshToken = new RefreshToken
        {
            Id = Guid.NewGuid(),
            UserId = user.Id,
            Token = refreshTokenValue,
            CreatedAtUtc = DateTime.UtcNow,
            ExpiresAtUtc = DateTime.UtcNow.AddDays(30)
        };

        await refreshTokenRepository.AddAsync(refreshToken, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        var response = new LoginResponse(user.Id,
            user.Email,
            user.FirstName,
            user.LastName,
            accessToken,
            refreshTokenValue);

        return Result.Success(response);
    }
}