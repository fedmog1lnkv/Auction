using Application.Abstractions.Storage;
using Application.Lots;
using Domain.Lots;

namespace Application.LotPhotos;

internal static class LotPhotoMappings
{
    extension(LotPhoto photo)
    {
        public LotCoverPhotoResponse ToCoverResponse(IObjectStorage objectStorage) =>
            new(
                objectStorage.GetPublicUrl(photo.ThumbKey),
                objectStorage.GetPublicUrl(photo.MediumKey));

        public LotPhotoResponse ToResponse(IObjectStorage objectStorage) =>
            new(
                photo.Id,
                photo.LotId,
                photo.SortOrder,
                new LotPhotoUrlsResponse(
                    objectStorage.GetPublicUrl(photo.ThumbKey),
                    objectStorage.GetPublicUrl(photo.MediumKey),
                    objectStorage.GetPublicUrl(photo.LargeKey)),
                photo.CreatedAt,
                photo.UpdatedAt);
    }
}