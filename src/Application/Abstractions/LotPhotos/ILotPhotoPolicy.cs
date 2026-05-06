namespace Application.Abstractions.LotPhotos;

public interface ILotPhotoPolicy
{
    int MaxLotPhotos { get; }
    long MaxPhotoSizeBytes { get; }
    int PresignedUrlTtlSeconds { get; }
    bool IsContentTypeAllowed(string contentType);
}