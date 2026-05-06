using Application.Abstractions.LotPhotos;

namespace Infrastructure.LotPhotos;

internal sealed class LotPhotoPolicy(int maxLotPhotos, long maxPhotoSizeBytes, int presignedUrlTtlSeconds)
    : ILotPhotoPolicy
{
    private static readonly HashSet<string> AllowedContentTypes = new(StringComparer.OrdinalIgnoreCase)
    {
        "image/jpeg",
        "image/png",
        "image/webp"
    };

    public int MaxLotPhotos { get; } = maxLotPhotos;
    public long MaxPhotoSizeBytes { get; } = maxPhotoSizeBytes;
    public int PresignedUrlTtlSeconds { get; } = presignedUrlTtlSeconds;

    public bool IsContentTypeAllowed(string contentType) => AllowedContentTypes.Contains(contentType.Trim());
}