using SharedKernel;

namespace Domain.Lots;

public sealed record LotStartedDomainEvent(
    Guid LotId,
    string Title,
    string? Description,
    Guid SellerId,
    decimal StartingPrice,
    decimal MinBidStep,
    decimal CurrentPrice,
    Guid? CurrentWinnerId,
    LotStatus Status,
    DateTime StartsAt,
    DateTime EndsAt,
    int Version,
    DateTime CreatedAt,
    DateTime UpdatedAt) : IDomainEvent;