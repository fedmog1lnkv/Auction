namespace Application.LotPhotos.ReorderLotPhotos;

public sealed record ReorderLotPhotosRequest(IReadOnlyList<Guid> PhotoIds);