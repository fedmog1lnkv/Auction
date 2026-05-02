using Domain.Users;

namespace Application.Abstractions.Authentication;

public interface IRefreshTokenRepository
{
    Task AddAsync(RefreshToken refreshToken, CancellationToken cancellationToken = default);
    Task<RefreshToken?> GetActiveByTokenAsync(string token, CancellationToken cancellationToken = default);
}