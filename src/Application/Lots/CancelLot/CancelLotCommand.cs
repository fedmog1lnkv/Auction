using MediatR;
using SharedKernel;

namespace Application.Lots.CancelLot;

public sealed record CancelLotCommand(Guid Id) : IRequest<Result<LotResponse>>;