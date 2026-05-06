namespace Application.Lots;

public sealed record LotPhotoResponse(
    Guid Id,
    Guid LotId,
    int SortOrder,
    LotPhotoUrlsResponse Urls,
    DateTime CreatedAt,
    DateTime UpdatedAt);