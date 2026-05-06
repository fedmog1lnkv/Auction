using SharedKernel;

namespace Domain.Lots;

public sealed class LotPhoto : Entity
{
    private LotPhoto()
    {
    }

    public Guid Id { get; private set; }
    public Guid LotId { get; private set; }
    public string ThumbKey { get; private set; } = string.Empty;
    public string MediumKey { get; private set; } = string.Empty;
    public string LargeKey { get; private set; } = string.Empty;
    public int SortOrder { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime UpdatedAt { get; private set; }

    public static Result<LotPhoto> Create(
        Guid id,
        Guid lotId,
        string thumbKey,
        string mediumKey,
        string largeKey,
        int sortOrder,
        DateTime utcNow)
    {
        if (sortOrder < 1)
            return Result.Failure<LotPhoto>(LotPhotoErrors.InvalidSortOrder);

        if (string.IsNullOrWhiteSpace(thumbKey) ||
            string.IsNullOrWhiteSpace(mediumKey) ||
            string.IsNullOrWhiteSpace(largeKey))
            return Result.Failure<LotPhoto>(LotPhotoErrors.InvalidKeys);

        var photo = new LotPhoto
        {
            Id = id,
            LotId = lotId,
            ThumbKey = thumbKey,
            MediumKey = mediumKey,
            LargeKey = largeKey,
            SortOrder = sortOrder,
            CreatedAt = utcNow,
            UpdatedAt = utcNow
        };

        return Result.Success(photo);
    }

    public Result SetSortOrder(int sortOrder, DateTime utcNow)
    {
        if (sortOrder < 1)
            return Result.Failure(LotPhotoErrors.InvalidSortOrder);

        SortOrder = sortOrder;
        UpdatedAt = utcNow;
        return Result.Success();
    }
}