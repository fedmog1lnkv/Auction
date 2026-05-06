namespace Application.LotPhotos.RequestLotPhotoUploadUrl;

public sealed record RequestLotPhotoUploadUrlResponse(
    string UploadId,
    string UploadUrl,
    int ExpiresInSeconds);