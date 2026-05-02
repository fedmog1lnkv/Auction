using System.Security.Cryptography;
using Application.Abstractions.Authentication;
using Application.Abstractions.Data;
using Domain.Users;
using MediatR;
using SharedKernel;

namespace Application.Auth.Refresh;

public sealed class RefreshTokenCommandHandler(
    IRefreshTokenRepository refreshTokenRepository,
    ITokenProvider tokenProvider,
    IUnitOfWork unitOfWork) : IRequestHandler<RefreshTokenCommand, Result<RefreshTokenResponse>>
{
    public async Task<Result<RefreshTokenResponse>> Handle(
        RefreshTokenCommand command,
        CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(command.RefreshToken))
            return Result.Failure<RefreshTokenResponse>(UserErrors.Unauthorized());

        var existing =
            await refreshTokenRepository.GetActiveByTokenAsync(command.RefreshToken, cancellationToken);
        if (existing is null)
            return Result.Failure<RefreshTokenResponse>(UserErrors.Unauthorized());

        existing.RevokedAtUtc = DateTime.UtcNow;

        var newRefreshTokenValue = Convert.ToBase64String(RandomNumberGenerator.GetBytes(64));
        var newRefreshToken = new RefreshToken
        {
            Id = Guid.NewGuid(),
            UserId = existing.UserId,
            Token = newRefreshTokenValue,
            CreatedAtUtc = DateTime.UtcNow,
            ExpiresAtUtc = DateTime.UtcNow.AddDays(30)
        };

        await refreshTokenRepository.AddAsync(newRefreshToken, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        var accessToken = tokenProvider.Create(existing.User);
        return Result.Success(new RefreshTokenResponse(accessToken, newRefreshTokenValue));
    }
}
