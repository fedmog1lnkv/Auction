namespace Application.Lots.UpdateLot;

public sealed record UpdateLotRequest(
    string? Title,
    string? Description,
    decimal? StartingPrice,
    decimal? MinBidStep,
    DateTime? StartsAt,
    DateTime? EndsAt);