using MediatR;
using SharedKernel;

namespace Application.Lots.CreateLot;

public sealed record CreateLotCommand(
    string Title,
    string? Description,
    decimal StartingPrice,
    decimal MinBidStep,
    DateTime StartsAt,
    DateTime EndsAt) : IRequest<Result<LotResponse>>;