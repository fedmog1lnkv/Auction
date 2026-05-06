using Application.Abstractions.Authentication;
using Application.Abstractions.Data;
using Application.Abstractions.LotPhotos;
using Application.Abstractions.Lots;
using Application.Abstractions.Storage;
using Application.Lots;
using Domain.Lots;
using MediatR;
using SharedKernel;

namespace Application.LotPhotos.ReorderLotPhotos;

public sealed class ReorderLotPhotosCommandHandler(
    ILotRepository lotRepository,
    ILotPhotoRepository lotPhotoRepository,
    IUnitOfWork unitOfWork,
    IUserContext userContext,
    IObjectStorage objectStorage)
    : IRequestHandler<ReorderLotPhotosCommand, Result<IReadOnlyList<LotPhotoResponse>>>
{
    public async Task<Result<IReadOnlyList<LotPhotoResponse>>> Handle(
        ReorderLotPhotosCommand command,
        CancellationToken cancellationToken)
    {
        var lot = await lotRepository.GetByIdAsync(command.LotId, cancellationToken);
        if (lot is null)
            return Result.Failure<IReadOnlyList<LotPhotoResponse>>(LotErrors.NotFound);

        if (!lot.CanBeManagedBy(userContext.UserId))
            return Result.Failure<IReadOnlyList<LotPhotoResponse>>(LotErrors.Forbidden);

        if (lot.Status != LotStatus.Draft)
            return Result.Failure<IReadOnlyList<LotPhotoResponse>>(LotPhotoErrors.PhotosCanBeManagedOnlyInDraft);

        if (command.PhotoIds.Count != command.PhotoIds.Distinct().Count())
            return Result.Failure<IReadOnlyList<LotPhotoResponse>>(LotPhotoErrors.InvalidReorderPayload);

        var photos = await lotPhotoRepository.GetByLotIdAsync(command.LotId, cancellationToken);

        if (photos.Count != command.PhotoIds.Count)
            return Result.Failure<IReadOnlyList<LotPhotoResponse>>(LotPhotoErrors.InvalidReorderPayload);

        var byId = photos.ToDictionary(x => x.Id, x => x);

        if (command.PhotoIds.Any(photoId => !byId.ContainsKey(photoId)))
            return Result.Failure<IReadOnlyList<LotPhotoResponse>>(LotPhotoErrors.InvalidReorderPayload);

        var utcNow = DateTime.UtcNow;

        foreach (var photo in photos)
        {
            var setTemporaryOrder = photo.SetSortOrder(photo.SortOrder + photos.Count, utcNow);
            if (setTemporaryOrder.IsFailure)
                return Result.Failure<IReadOnlyList<LotPhotoResponse>>(setTemporaryOrder.Error);
        }

        await unitOfWork.SaveChangesAsync(cancellationToken);

        for (var index = 0; index < command.PhotoIds.Count; index++)
        {
            var photo = byId[command.PhotoIds[index]];
            var setFinalOrder = photo.SetSortOrder(index + 1, utcNow);
            if (setFinalOrder.IsFailure)
                return Result.Failure<IReadOnlyList<LotPhotoResponse>>(setFinalOrder.Error);
        }

        await unitOfWork.SaveChangesAsync(cancellationToken);

        IReadOnlyList<LotPhotoResponse> response = command.PhotoIds
            .Select(photoId => byId[photoId].ToResponse(objectStorage))
            .ToList();

        return Result.Success(response);
    }
}