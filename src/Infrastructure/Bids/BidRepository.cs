using System.Data;
using Application.Abstractions.Bids;
using Domain.Bids;
using Domain.Lots;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Npgsql;
using SharedKernel;

namespace Infrastructure.Bids;

internal sealed class BidRepository(
    ApplicationDbContext dbContext,
    ILogger<BidRepository> logger) : IBidRepository
{
    public async Task<Result<BidPlacement>> PlaceAsync(
        Guid lotId,
        Guid bidderId,
        decimal amount,
        CancellationToken cancellationToken = default)
    {
        await using var transaction = await dbContext.Database.BeginTransactionAsync(
            IsolationLevel.ReadCommitted,
            cancellationToken);

        var lot = await dbContext.Lots
            .FromSqlInterpolated($"SELECT * FROM lots WHERE id = {lotId} FOR UPDATE")
            .AsTracking()
            .SingleOrDefaultAsync(cancellationToken);

        if (lot is null)
            return Result.Failure<BidPlacement>(LotErrors.NotFound);

        var bidResult = lot.PlaceBid(Guid.NewGuid(), bidderId, amount, DateTime.UtcNow);
        if (bidResult.IsFailure)
            return Result.Failure<BidPlacement>(bidResult.Error);

        await dbContext.Bids.AddAsync(bidResult.Value, cancellationToken);

        try
        {
            await dbContext.SaveChangesAsync(cancellationToken);
            await transaction.CommitAsync(cancellationToken);
        }
        catch (DbUpdateConcurrencyException ex)
        {
            logger.LogWarning(ex, "Concurrent lot update detected while placing bid for lot {LotId}", lotId);
            return Result.Failure<BidPlacement>(BidErrors.ConcurrentChange);
        }
        catch (PostgresException ex) when (IsConcurrentWriteFailure(ex))
        {
            logger.LogWarning(ex, "PostgreSQL concurrent write failure while placing bid for lot {LotId}", lotId);
            return Result.Failure<BidPlacement>(BidErrors.ConcurrentChange);
        }

        return Result.Success(new BidPlacement(bidResult.Value, lot));
    }

    private static bool IsConcurrentWriteFailure(PostgresException exception) =>
        exception.SqlState is PostgresErrorCodes.DeadlockDetected
            or PostgresErrorCodes.LockNotAvailable
            or PostgresErrorCodes.SerializationFailure;
}