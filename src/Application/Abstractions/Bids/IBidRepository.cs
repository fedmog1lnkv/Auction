using SharedKernel;

namespace Application.Abstractions.Bids;

public interface IBidRepository
{
    Task<Result<BidPlacement>> PlaceAsync(
        Guid lotId,
        Guid bidderId,
        decimal amount,
        CancellationToken cancellationToken = default);
}