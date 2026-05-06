using Application.Lots;
using MediatR;
using SharedKernel;

namespace Application.LotPhotos.GetLotPhotos;

public sealed record GetLotPhotosQuery(Guid LotId) : IRequest<Result<IReadOnlyList<LotPhotoResponse>>>;