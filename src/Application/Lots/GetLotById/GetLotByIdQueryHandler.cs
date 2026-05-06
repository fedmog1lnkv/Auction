using Application.Abstractions.LotPhotos;
using Application.Abstractions.Lots;
using Application.Abstractions.Storage;
using Application.LotPhotos;
using Domain.Lots;
using MediatR;
using SharedKernel;

namespace Application.Lots.GetLotById;

public sealed class GetLotByIdQueryHandler(
    ILotRepository lotRepository,
    ILotPhotoRepository lotPhotoRepository,
    IObjectStorage objectStorage)
    : IRequestHandler<GetLotByIdQuery, Result<LotResponse>>
{
    public async Task<Result<LotResponse>> Handle(GetLotByIdQuery query, CancellationToken cancellationToken)
    {
        var lot = await lotRepository.GetByIdAsync(query.Id, cancellationToken);
        if (lot is null)
            return Result.Failure<LotResponse>(LotErrors.NotFound);

        IReadOnlyList<LotPhotoResponse> photos = (await lotPhotoRepository.GetByLotIdAsync(lot.Id, cancellationToken))
            .Select(photo => photo.ToResponse(objectStorage))
            .ToList();

        var coverPhoto = photos.FirstOrDefault() is { } firstPhoto
            ? new LotCoverPhotoResponse(firstPhoto.Urls.Thumb, firstPhoto.Urls.Medium)
            : null;

        return Result.Success(LotResponse.FromDomain(lot, coverPhoto, photos));
    }
}