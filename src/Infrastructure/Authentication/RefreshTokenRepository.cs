using Application.Abstractions.Authentication;
using Domain.Users;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Authentication;

internal sealed class RefreshTokenRepository(ApplicationDbContext dbContext) : IRefreshTokenRepository
{
    public async Task AddAsync(RefreshToken refreshToken, CancellationToken cancellationToken = default) =>
        await dbContext.RefreshTokens.AddAsync(refreshToken, cancellationToken);

    public Task<RefreshToken?> GetActiveByTokenAsync(string token, CancellationToken cancellationToken = default)
    {
        var now = DateTime.UtcNow;

        return dbContext.RefreshTokens
            .Include(x => x.User)
            .FirstOrDefaultAsync(
                x => x.Token == token &&
                     x.RevokedAtUtc == null &&
                     x.ExpiresAtUtc > now,
                cancellationToken);
    }
}