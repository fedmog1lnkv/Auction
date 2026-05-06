using MediatR;
using SharedKernel;

namespace Application.LotPhotos.DeleteLotPhoto;

public sealed record DeleteLotPhotoCommand(Guid LotId, Guid PhotoId) : IRequest<Result>;