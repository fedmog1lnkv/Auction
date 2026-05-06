using Application.Abstractions.LotPhotos;
using Application.Abstractions.Lots;
using Application.Abstractions.Storage;
using Application.Common.Pagination;
using Application.LotPhotos;
using Domain.Lots;
using MediatR;
using SharedKernel;

namespace Application.Lots.GetLots;

public sealed class GetLotsQueryHandler(
    ILotRepository lotRepository,
    ILotPhotoRepository lotPhotoRepository,
    IObjectStorage objectStorage)
    : IRequestHandler<GetLotsQuery, Result<PagedResponse<LotResponse>>>
{
    public async Task<Result<PagedResponse<LotResponse>>> Handle(
        GetLotsQuery query,
        CancellationToken cancellationToken)
    {
        var page = query.Page < 1 ? 1 : query.Page;
        var limit = query.Limit <= 0 ? 20 : Math.Min(query.Limit, 100);
        var skip = (page - 1) * limit;

        var lots = await lotRepository.GetListAsync(
            query.Status,
            query.SellerId,
            skip,
            limit,
            cancellationToken);

        var total = await lotRepository.CountAsync(query.Status, query.SellerId, cancellationToken);

        var lotIds = lots.Select(x => x.Id).ToList();
        var coverPhotos = await lotPhotoRepository.GetCoverPhotosByLotIdsAsync(lotIds, cancellationToken);

        var items = lots
            .Select(lot =>
            {
                var coverPhoto = coverPhotos.TryGetValue(lot.Id, out LotPhoto? photo)
                    ? photo.ToCoverResponse(objectStorage)
                    : null;

                return LotResponse.FromDomain(lot, coverPhoto, null);
            })
            .ToList();

        var response = new PagedResponse<LotResponse>(
            items,
            page,
            limit,
            total);

        return Result.Success(response);
    }
}