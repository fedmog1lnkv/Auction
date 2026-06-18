using Application.Common.Pagination;
using MediatR;
using SharedKernel;

namespace Application.Bids.GetLotBids;

public sealed record GetLotBidsQuery(
    Guid LotId,
    int Page = 1,
    int Limit = 20) : IRequest<Result<PagedResponse<BidListItemResponse>>>;