using Application.Lots;
using MediatR;
using SharedKernel;

namespace Application.LotPhotos.CompleteLotPhotoUpload;

public sealed record CompleteLotPhotoUploadCommand(Guid LotId, string UploadId)
    : IRequest<Result<LotPhotoResponse>>;