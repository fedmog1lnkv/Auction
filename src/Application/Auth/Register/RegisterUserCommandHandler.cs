using System.Security.Cryptography;
using Application.Abstractions.Authentication;
using Application.Abstractions.Data;
using Application.Abstractions.Users;
using Domain.Users;
using MediatR;
using SharedKernel;

namespace Application.Auth.Register;

public sealed class RegisterUserCommandHandler(
    IUserRepository userRepository,
    IPasswordHasher passwordHasher,
    ITokenProvider tokenProvider,
    IRefreshTokenRepository refreshTokenRepository,
    IUnitOfWork unitOfWork) : IRequestHandler<RegisterUserCommand, Result<RegisterResponse>>
{
    public async Task<Result<RegisterResponse>> Handle(RegisterUserCommand command, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(command.Password))
            return Result.Failure<RegisterResponse>(Error.Failure("Users.InvalidPassword", "Password is required."));

        var existingUser = await userRepository.GetByEmailAsync(command.Email, cancellationToken);
        if (existingUser is not null)
            return Result.Failure<RegisterResponse>(UserErrors.EmailNotUnique);

        var passwordHash = passwordHasher.Hash(command.Password);
        var userResult = User.Create(
            Guid.NewGuid(),
            command.Email,
            command.FirstName,
            command.LastName,
            passwordHash);
        if (userResult.IsFailure)
            return Result.Failure<RegisterResponse>(userResult.Error);

        var user = userResult.Value;

        await userRepository.AddAsync(user, cancellationToken);

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

        return Result.Success(new RegisterResponse(
            user.Id,
            user.Email,
            user.FirstName,
            user.LastName,
            accessToken,
            refreshTokenValue));
    }
}