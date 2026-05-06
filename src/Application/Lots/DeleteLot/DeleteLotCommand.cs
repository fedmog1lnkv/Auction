using MediatR;
using SharedKernel;

namespace Application.Lots.DeleteLot;

public sealed record DeleteLotCommand(Guid Id) : IRequest<Result>;