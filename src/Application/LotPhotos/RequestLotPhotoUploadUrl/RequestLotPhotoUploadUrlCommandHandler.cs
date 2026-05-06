using Application.Abstractions.Authentication;
using Application.Abstractions.LotPhotos;
using Application.Abstractions.Lots;
using Application.Abstractions.Storage;
using Domain.Lots;
using MediatR;
using SharedKernel;

namespace Application.LotPhotos.RequestLotPhotoUploadUrl;

public sealed class RequestLotPhotoUploadUrlCommandHandler(
    ILotRepository lotRepository,
    ILotPhotoRepository lotPhotoRepository,
    IUserContext userContext,
    ILotPhotoPolicy lotPhotoPolicy,
    IObjectStorage objectStorage)
    : IRequestHandler<RequestLotPhotoUploadUrlCommand, Result<RequestLotPhotoUploadUrlResponse>>
{
    public async Task<Result<RequestLotPhotoUploadUrlResponse>> Handle(
        RequestLotPhotoUploadUrlCommand command,
        CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(command.ContentType))
            return Result.Failure<RequestLotPhotoUploadUrlResponse>(LotPhotoErrors.InvalidContentType);

        var lot = await lotRepository.GetByIdAsync(command.LotId, cancellationToken);
        if (lot is null)
            return Result.Failure<RequestLotPhotoUploadUrlResponse>(LotErrors.NotFound);

        if (!lot.CanBeManagedBy(userContext.UserId))
            return Result.Failure<RequestLotPhotoUploadUrlResponse>(LotErrors.Forbidden);

        if (lot.Status != LotStatus.Draft)
            return Result.Failure<RequestLotPhotoUploadUrlResponse>(LotPhotoErrors.PhotosCanBeManagedOnlyInDraft);

        var contentType = command.ContentType.Trim();

        if (!lotPhotoPolicy.IsContentTypeAllowed(contentType))
            return Result.Failure<RequestLotPhotoUploadUrlResponse>(LotPhotoErrors.InvalidContentType);

        var photosCount = await lotPhotoRepository.CountByLotIdAsync(command.LotId, cancellationToken);
        if (photosCount >= lotPhotoPolicy.MaxLotPhotos)
            return Result.Failure<RequestLotPhotoUploadUrlResponse>(LotPhotoErrors.TooManyPhotos);

        var uploadId = Guid.NewGuid().ToString("N");
        var tempKey = $"lots/{command.LotId}/uploads/{uploadId}";
        var uploadUrl = await objectStorage.GeneratePresignedUploadUrlAsync(
            tempKey,
            contentType,
            TimeSpan.FromSeconds(lotPhotoPolicy.PresignedUrlTtlSeconds),
            cancellationToken);

        return Result.Success(new RequestLotPhotoUploadUrlResponse(
            uploadId,
            uploadUrl,
            lotPhotoPolicy.PresignedUrlTtlSeconds));
    }
}