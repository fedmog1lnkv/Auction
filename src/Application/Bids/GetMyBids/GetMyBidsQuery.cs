using Application.Common.Pagination;
using MediatR;
using SharedKernel;

namespace Application.Bids.GetMyBids;

public sealed record GetMyBidsQuery(
    int Page = 1,
    int Limit = 20) : IRequest<Result<PagedResponse<BidListItemResponse>>>;