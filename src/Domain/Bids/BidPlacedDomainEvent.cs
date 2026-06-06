using SharedKernel;

namespace Domain.Bids;

public sealed record BidPlacedDomainEvent(
    Guid BidId,
    Guid LotId,
    Guid BidderId,
    decimal Amount,
    decimal CurrentPrice,
    Guid CurrentWinnerId,
    int LotVersion,
    DateTime BidCreatedAt,
    DateTime LotUpdatedAt) : IDomainEvent;