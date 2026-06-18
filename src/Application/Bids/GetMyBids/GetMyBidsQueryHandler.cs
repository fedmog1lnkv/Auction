using Application.Abstractions.Authentication;
using Application.Abstractions.Bids;
using Application.Common.Pagination;
using MediatR;
using SharedKernel;

namespace Application.Bids.GetMyBids;

public sealed class GetMyBidsQueryHandler(
    IBidRepository bidRepository,
    IUserContext userContext)
    : IRequestHandler<GetMyBidsQuery, Result<PagedResponse<BidListItemResponse>>>
{
    public async Task<Result<PagedResponse<BidListItemResponse>>> Handle(
        GetMyBidsQuery query,
        CancellationToken cancellationToken)
    {
        var page = query.Page < 1 ? 1 : query.Page;
        var limit = query.Limit <= 0 ? 20 : Math.Min(query.Limit, 100);
        var skip = (page - 1) * limit;

        var bids = await bidRepository.GetByBidderIdAsync(userContext.UserId, skip, limit, cancellationToken);
        var total = await bidRepository.CountByBidderIdAsync(userContext.UserId, cancellationToken);
        var items = bids.Select(BidListItemResponse.FromDomain).ToList();

        return Result.Success(new PagedResponse<BidListItemResponse>(items, page, limit, total));
    }
}