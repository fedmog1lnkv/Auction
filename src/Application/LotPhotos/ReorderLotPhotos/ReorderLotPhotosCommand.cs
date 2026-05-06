using Application.Lots;
using MediatR;
using SharedKernel;

namespace Application.LotPhotos.ReorderLotPhotos;

public sealed record ReorderLotPhotosCommand(Guid LotId, IReadOnlyList<Guid> PhotoIds)
    : IRequest<Result<IReadOnlyList<LotPhotoResponse>>>;