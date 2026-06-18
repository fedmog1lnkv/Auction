using Application.Abstractions.Bids;
using Application.Abstractions.Lots;
using Application.Common.Pagination;
using Domain.Lots;
using MediatR;
using SharedKernel;

namespace Application.Bids.GetLotBids;

public sealed class GetLotBidsQueryHandler(
    IBidRepository bidRepository,
    ILotRepository lotRepository)
    : IRequestHandler<GetLotBidsQuery, Result<PagedResponse<BidListItemResponse>>>
{
    public async Task<Result<PagedResponse<BidListItemResponse>>> Handle(
        GetLotBidsQuery query,
        CancellationToken cancellationToken)
    {
        var lot = await lotRepository.GetByIdAsync(query.LotId, cancellationToken);
        if (lot is null)
            return Result.Failure<PagedResponse<BidListItemResponse>>(LotErrors.NotFound);

        var page = query.Page < 1 ? 1 : query.Page;
        var limit = query.Limit <= 0 ? 20 : Math.Min(query.Limit, 100);
        var skip = (page - 1) * limit;

        var bids = await bidRepository.GetByLotIdAsync(query.LotId, skip, limit, cancellationToken);
        var total = await bidRepository.CountByLotIdAsync(query.LotId, cancellationToken);
        var items = bids.Select(BidListItemResponse.FromDomain).ToList();

        return Result.Success(new PagedResponse<BidListItemResponse>(items, page, limit, total));
    }
}