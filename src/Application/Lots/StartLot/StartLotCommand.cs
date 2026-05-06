using MediatR;
using SharedKernel;

namespace Application.Lots.StartLot;

public sealed record StartLotCommand(Guid Id) : IRequest<Result<LotResponse>>;