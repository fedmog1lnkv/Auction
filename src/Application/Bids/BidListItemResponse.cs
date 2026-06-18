using Domain.Bids;

namespace Application.Bids;

public sealed record BidListItemResponse(
    Guid Id,
    Guid LotId,
    Guid BidderId,
    decimal Amount,
    DateTime CreatedAt)
{
    public static BidListItemResponse FromDomain(Bid bid) =>
        new(
            bid.Id,
            bid.LotId,
            bid.BidderId,
            bid.Amount,
            bid.CreatedAt);
}