using Application.Abstractions.LotPhotos;
using Application.Abstractions.Lots;
using Application.Abstractions.Storage;
using Application.Lots;
using Domain.Lots;
using MediatR;
using SharedKernel;

namespace Application.LotPhotos.GetLotPhotos;

public sealed class GetLotPhotosQueryHandler(
    ILotRepository lotRepository,
    ILotPhotoRepository lotPhotoRepository,
    IObjectStorage objectStorage)
    : IRequestHandler<GetLotPhotosQuery, Result<IReadOnlyList<LotPhotoResponse>>>
{
    public async Task<Result<IReadOnlyList<LotPhotoResponse>>> Handle(
        GetLotPhotosQuery query,
        CancellationToken cancellationToken)
    {
        var lot = await lotRepository.GetByIdAsync(query.LotId, cancellationToken);
        if (lot is null)
            return Result.Failure<IReadOnlyList<LotPhotoResponse>>(LotErrors.NotFound);

        IReadOnlyList<LotPhoto> photos = await lotPhotoRepository.GetByLotIdAsync(query.LotId, cancellationToken);

        IReadOnlyList<LotPhotoResponse> response = photos
            .Select(photo => photo.ToResponse(objectStorage))
            .ToList();

        return Result.Success(response);
    }
}