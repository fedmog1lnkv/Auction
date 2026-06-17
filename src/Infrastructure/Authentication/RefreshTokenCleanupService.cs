using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Infrastructure.Authentication;

internal sealed class RefreshTokenCleanupService(
    IServiceScopeFactory scopeFactory,
    IOptions<RefreshTokenCleanupOptions> options,
    ILogger<RefreshTokenCleanupService> logger) : BackgroundService
{
    private readonly RefreshTokenCleanupOptions _options = options.Value;

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        if (_options.IntervalHours <= 0)
        {
            logger.LogWarning("Refresh token cleanup is disabled because interval is <= 0");
            return;
        }

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                await CleanupAsync(stoppingToken);
            }
            catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
            {
                break;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Refresh token cleanup failed");
            }

            try
            {
                await Task.Delay(TimeSpan.FromHours(_options.IntervalHours), stoppingToken);
            }
            catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
            {
                break;
            }
        }
    }

    private async Task CleanupAsync(CancellationToken cancellationToken)
    {
        using var scope = scopeFactory.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        var now = DateTime.UtcNow;
        var revokedBefore = now.AddDays(-_options.RevokedRetentionDays);
        var expiredBefore = now.AddDays(-_options.ExpiredRetentionDays);

        var deleted = await dbContext.RefreshTokens
            .Where(x =>
                (x.RevokedAtUtc != null && x.RevokedAtUtc < revokedBefore) ||
                (x.ExpiresAtUtc < expiredBefore))
            .ExecuteDeleteAsync(cancellationToken);

        if (deleted > 0) logger.LogInformation("Refresh token cleanup deleted {DeletedCount} rows", deleted);
    }
}
