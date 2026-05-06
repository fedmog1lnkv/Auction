using MediatR;
using SharedKernel;

namespace Application.Lots.FinishLot;

public sealed record FinishLotCommand(Guid Id) : IRequest<Result<LotResponse>>;