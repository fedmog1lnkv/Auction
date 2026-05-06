using MediatR;
using SharedKernel;

namespace Application.LotPhotos.RequestLotPhotoUploadUrl;

public sealed record RequestLotPhotoUploadUrlCommand(Guid LotId, string ContentType)
    : IRequest<Result<RequestLotPhotoUploadUrlResponse>>;