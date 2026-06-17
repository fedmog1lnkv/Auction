using Domain.Bids;
using Domain.Lots;

namespace Application.Bids;

public sealed record BidResponse(
    Guid Id,
    Guid LotId,
    Guid BidderId,
    decimal Amount,
    decimal CurrentPrice,
    Guid CurrentWinnerId,
    int LotVersion,
    DateTime CreatedAt)
{
    public static BidResponse FromDomain(Bid bid, Lot lot) =>
        new(
            bid.Id,
            bid.LotId,
            bid.BidderId,
            bid.Amount,
            lot.CurrentPrice,
            lot.CurrentWinnerId ?? bid.BidderId,
            lot.Version,
            bid.CreatedAt);
}