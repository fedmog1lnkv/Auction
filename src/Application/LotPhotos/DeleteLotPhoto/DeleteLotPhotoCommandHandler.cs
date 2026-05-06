using Application.Abstractions.Authentication;
using Application.Abstractions.Data;
using Application.Abstractions.LotPhotos;
using Application.Abstractions.Lots;
using Application.Abstractions.Storage;
using Domain.Lots;
using MediatR;
using SharedKernel;

namespace Application.LotPhotos.DeleteLotPhoto;

public sealed class DeleteLotPhotoCommandHandler(
    ILotRepository lotRepository,
    ILotPhotoRepository lotPhotoRepository,
    IUnitOfWork unitOfWork,
    IUserContext userContext,
    IObjectStorage objectStorage)
    : IRequestHandler<DeleteLotPhotoCommand, Result>
{
    public async Task<Result> Handle(DeleteLotPhotoCommand command, CancellationToken cancellationToken)
    {
        var lot = await lotRepository.GetByIdAsync(command.LotId, cancellationToken);
        if (lot is null)
            return Result.Failure(LotErrors.NotFound);

        if (!lot.CanBeManagedBy(userContext.UserId))
            return Result.Failure(LotErrors.Forbidden);

        if (lot.Status != LotStatus.Draft)
            return Result.Failure(LotPhotoErrors.PhotosCanBeManagedOnlyInDraft);

        var photos = await lotPhotoRepository.GetByLotIdAsync(command.LotId, cancellationToken);
        var targetPhoto = photos.FirstOrDefault(photo => photo.Id == command.PhotoId);
        if (targetPhoto is null)
            return Result.Failure(LotPhotoErrors.PhotoNotFound);

        await objectStorage.DeleteAsync(targetPhoto.ThumbKey, cancellationToken);
        await objectStorage.DeleteAsync(targetPhoto.MediumKey, cancellationToken);
        await objectStorage.DeleteAsync(targetPhoto.LargeKey, cancellationToken);

        lotPhotoRepository.Remove(targetPhoto);

        var utcNow = DateTime.UtcNow;
        var remainingPhotos = photos
            .Where(photo => photo.Id != command.PhotoId)
            .OrderBy(photo => photo.SortOrder)
            .ToList();

        for (var index = 0; index < remainingPhotos.Count; index++)
        {
            var setSortOrderResult = remainingPhotos[index].SetSortOrder(index + 1, utcNow);
            if (setSortOrderResult.IsFailure)
                return setSortOrderResult;
        }

        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}