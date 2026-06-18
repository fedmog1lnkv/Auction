using Domain.Bids;
using SharedKernel;

namespace Application.Abstractions.Bids;

public interface IBidRepository
{
    Task<Result<BidPlacement>> PlaceAsync(
        Guid lotId,
        Guid bidderId,
        decimal amount,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<Bid>> GetByLotIdAsync(
        Guid lotId,
        int skip,
        int take,
        CancellationToken cancellationToken = default);

    Task<int> CountByLotIdAsync(Guid lotId, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<Bid>> GetByBidderIdAsync(
        Guid bidderId,
        int skip,
        int take,
        CancellationToken cancellationToken = default);

    Task<int> CountByBidderIdAsync(Guid bidderId, CancellationToken cancellationToken = default);
}