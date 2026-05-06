namespace Application.Lots.CreateLot;

public sealed record CreateLotRequest(
    string Title,
    string? Description,
    decimal StartingPrice,
    decimal MinBidStep,
    DateTime StartsAt,
    DateTime EndsAt);