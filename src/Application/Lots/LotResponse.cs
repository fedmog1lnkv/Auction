using Domain.Lots;

namespace Application.Lots;

public sealed record LotResponse(
    Guid Id,
    string Title,
    string? Description,
    Guid SellerId,
    decimal StartingPrice,
    decimal MinBidStep,
    decimal CurrentPrice,
    Guid? CurrentWinnerId,
    string Status,
    DateTime StartsAt,
    DateTime EndsAt,
    int Version,
    DateTime CreatedAt,
    DateTime UpdatedAt,
    LotCoverPhotoResponse? CoverPhoto,
    IReadOnlyList<LotPhotoResponse>? Photos)
{
    public static LotResponse FromDomain(
        Lot lot,
        LotCoverPhotoResponse? coverPhoto = null,
        IReadOnlyList<LotPhotoResponse>? photos = null) =>
        new(
            lot.Id,
            lot.Title,
            lot.Description,
            lot.SellerId,
            lot.StartingPrice,
            lot.MinBidStep,
            lot.CurrentPrice,
            lot.CurrentWinnerId,
            lot.Status.ToString().ToUpperInvariant(),
            lot.StartsAt,
            lot.EndsAt,
            lot.Version,
            lot.CreatedAt,
            lot.UpdatedAt,
            coverPhoto,
            photos);
}