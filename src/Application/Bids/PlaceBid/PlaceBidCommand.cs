using MediatR;
using SharedKernel;

namespace Application.Bids.PlaceBid;

public sealed record PlaceBidCommand(Guid LotId, decimal Amount) : IRequest<Result<BidResponse>>;