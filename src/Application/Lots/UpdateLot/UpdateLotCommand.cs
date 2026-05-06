using MediatR;
using SharedKernel;

namespace Application.Lots.UpdateLot;

public sealed record UpdateLotCommand(
    Guid Id,
    string? Title,
    string? Description,
    decimal? StartingPrice,
    decimal? MinBidStep,
    DateTime? StartsAt,
    DateTime? EndsAt) : IRequest<Result<LotResponse>>;