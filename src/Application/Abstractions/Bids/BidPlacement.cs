using Domain.Bids;
using Domain.Lots;

namespace Application.Abstractions.Bids;

public sealed record BidPlacement(Bid Bid, Lot Lot);