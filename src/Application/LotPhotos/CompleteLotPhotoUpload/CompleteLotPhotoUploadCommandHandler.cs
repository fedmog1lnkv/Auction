using Application.Abstractions.Authentication;
using Application.Abstractions.Data;
using Application.Abstractions.LotPhotos;
using Application.Abstractions.Lots;
using Application.Abstractions.Storage;
using Application.Lots;
using Domain.Lots;
using MediatR;
using SharedKernel;

namespace Application.LotPhotos.CompleteLotPhotoUpload;

public sealed class CompleteLotPhotoUploadCommandHandler(
    ILotRepository lotRepository,
    ILotPhotoRepository lotPhotoRepository,
    IUnitOfWork unitOfWork,
    IUserContext userContext,
    ILotPhotoPolicy lotPhotoPolicy,
    IObjectStorage objectStorage,
    IImageVariantGenerator imageVariantGenerator)
    : IRequestHandler<CompleteLotPhotoUploadCommand, Result<LotPhotoResponse>>
{
    public async Task<Result<LotPhotoResponse>> Handle(
        CompleteLotPhotoUploadCommand command,
        CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(command.UploadId) || !Guid.TryParse(command.UploadId, out _))
            return Result.Failure<LotPhotoResponse>(LotPhotoErrors.InvalidUploadId);

        var lot = await lotRepository.GetByIdAsync(command.LotId, cancellationToken);
        if (lot is null)
            return Result.Failure<LotPhotoResponse>(LotErrors.NotFound);

        if (!lot.CanBeManagedBy(userContext.UserId))
            return Result.Failure<LotPhotoResponse>(LotErrors.Forbidden);

        if (lot.Status != LotStatus.Draft)
            return Result.Failure<LotPhotoResponse>(LotPhotoErrors.PhotosCanBeManagedOnlyInDraft);

        var photosCount = await lotPhotoRepository.CountByLotIdAsync(command.LotId, cancellationToken);
        if (photosCount >= lotPhotoPolicy.MaxLotPhotos)
            return Result.Failure<LotPhotoResponse>(LotPhotoErrors.TooManyPhotos);

        var uploadId = command.UploadId;
        var tempKey = $"lots/{command.LotId}/uploads/{uploadId}";
        if (!await objectStorage.ExistsAsync(tempKey, cancellationToken))
            return Result.Failure<LotPhotoResponse>(LotPhotoErrors.UploadNotFound);

        var (content, contentType, sizeBytes) = await objectStorage.DownloadAsync(tempKey, cancellationToken);

        try
        {
            if (!string.IsNullOrWhiteSpace(contentType) &&
                !lotPhotoPolicy.IsContentTypeAllowed(contentType))

                return Result.Failure<LotPhotoResponse>(LotPhotoErrors.InvalidContentType);

            if (sizeBytes > lotPhotoPolicy.MaxPhotoSizeBytes)
                return Result.Failure<LotPhotoResponse>(LotPhotoErrors.FileTooLarge);

            ImageVariants variants;
            try
            {
                variants = await imageVariantGenerator.CreateWebpVariantsAsync(content,
                    cancellationToken);
            }
            catch (NotSupportedException)
            {
                return Result.Failure<LotPhotoResponse>(LotPhotoErrors.InvalidContentType);
            }
            catch
            {
                return Result.Failure<LotPhotoResponse>(LotPhotoErrors.InvalidImage);
            }

            var photoId = Guid.NewGuid();
            var thumbKey = $"lots/{command.LotId}/photos/{photoId}/thumb.webp";
            var mediumKey = $"lots/{command.LotId}/photos/{photoId}/medium.webp";
            var largeKey = $"lots/{command.LotId}/photos/{photoId}/large.webp";
            var utcNow = DateTime.UtcNow;

            var photoResult = LotPhoto.Create(
                photoId,
                command.LotId,
                thumbKey,
                mediumKey,
                largeKey,
                photosCount + 1,
                utcNow);

            if (photoResult.IsFailure)
            {
                return Result.Failure<LotPhotoResponse>(photoResult.Error);
            }

            try
            {
                await objectStorage.UploadAsync(thumbKey, variants.ThumbWebp, "image/webp", cancellationToken);
                await objectStorage.UploadAsync(mediumKey, variants.MediumWebp, "image/webp", cancellationToken);
                await objectStorage.UploadAsync(largeKey, variants.LargeWebp, "image/webp", cancellationToken);

                await lotPhotoRepository.AddAsync(photoResult.Value, cancellationToken);
                await unitOfWork.SaveChangesAsync(cancellationToken);
            }
            catch
            {
                await objectStorage.DeleteAsync(thumbKey, cancellationToken);
                await objectStorage.DeleteAsync(mediumKey, cancellationToken);
                await objectStorage.DeleteAsync(largeKey, cancellationToken);
                throw;
            }

            return Result.Success(photoResult.Value.ToResponse(objectStorage));
        }
        finally
        {
            await objectStorage.DeleteAsync(tempKey, cancellationToken);
        }
    }
}