using MediatR;
using SharedKernel;

namespace Application.Lots.GetLotById;

public sealed record GetLotByIdQuery(Guid Id) : IRequest<Result<LotResponse>>;