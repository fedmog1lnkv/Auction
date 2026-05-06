using SharedKernel;

namespace Domain.Lots;

public sealed class Lot : Entity
{
    private Lot()
    {
    }

    public Guid Id { get; private set; }
    public string Title { get; private set; } = string.Empty;
    public string? Description { get; private set; }
    public Guid SellerId { get; private set; }
    public decimal StartingPrice { get; private set; }
    public decimal MinBidStep { get; private set; }
    public decimal CurrentPrice { get; private set; }
    public Guid? CurrentWinnerId { get; private set; }
    public LotStatus Status { get; private set; }
    public DateTime StartsAt { get; private set; }
    public DateTime EndsAt { get; private set; }
    public int Version { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime UpdatedAt { get; private set; }

    public static Result<Lot> Create(
        Guid id,
        Guid sellerId,
        string title,
        string? description,
        decimal startingPrice,
        decimal minBidStep,
        DateTime startsAt,
        DateTime endsAt,
        DateTime utcNow)
    {
        var validation = ValidateDraftData(title, description, startingPrice, minBidStep, startsAt, endsAt);
        if (validation.IsFailure)
            return Result.Failure<Lot>(validation.Error);

        var lot = new Lot
        {
            Id = id,
            SellerId = sellerId,
            Title = title.Trim(),
            Description = string.IsNullOrWhiteSpace(description) ? null : description.Trim(),
            StartingPrice = startingPrice,
            MinBidStep = minBidStep,
            CurrentPrice = startingPrice,
            CurrentWinnerId = null,
            Status = LotStatus.Draft,
            StartsAt = startsAt,
            EndsAt = endsAt,
            Version = 1,
            CreatedAt = utcNow,
            UpdatedAt = utcNow
        };

        return Result.Success(lot);
    }

    public Result UpdateDraft(
        string title,
        string? description,
        decimal startingPrice,
        decimal minBidStep,
        DateTime startsAt,
        DateTime endsAt,
        DateTime utcNow)
    {
        if (Status != LotStatus.Draft)
            return Result.Failure(LotErrors.UpdateAllowedOnlyInDraft);

        var validation = ValidateDraftData(title, description, startingPrice, minBidStep, startsAt, endsAt);
        if (validation.IsFailure)
            return validation;

        Title = title.Trim();
        Description = string.IsNullOrWhiteSpace(description) ? null : description.Trim();
        StartingPrice = startingPrice;
        MinBidStep = minBidStep;
        StartsAt = startsAt;
        EndsAt = endsAt;
        CurrentPrice = startingPrice;
        Touch(utcNow);

        return Result.Success();
    }

    public Result Start(DateTime utcNow)
    {
        if (Status != LotStatus.Draft)
            return Result.Failure(LotErrors.InvalidStatusTransition(Status, LotStatus.Active));

        var validation = ValidateDraftData(Title, Description, StartingPrice, MinBidStep, StartsAt, EndsAt);
        if (validation.IsFailure)
            return validation;

        if (EndsAt <= utcNow)
            return Result.Failure(LotErrors.EndsAtMustBeInFuture);

        Status = LotStatus.Active;
        Touch(utcNow);

        Raise(new LotStartedDomainEvent(
            Id,
            Title,
            Description,
            SellerId,
            StartingPrice,
            MinBidStep,
            CurrentPrice,
            CurrentWinnerId,
            Status,
            StartsAt,
            EndsAt,
            Version,
            CreatedAt,
            UpdatedAt));

        return Result.Success();
    }

    public Result Finish(DateTime utcNow)
    {
        if (Status != LotStatus.Active)
            return Result.Failure(LotErrors.InvalidStatusTransition(Status, LotStatus.Finished));

        Status = LotStatus.Finished;
        Touch(utcNow);
        return Result.Success();
    }

    public Result Cancel(DateTime utcNow)
    {
        if (Status != LotStatus.Draft && Status != LotStatus.Active)
            return Result.Failure(LotErrors.InvalidStatusTransition(Status, LotStatus.Cancelled));

        Status = LotStatus.Cancelled;
        Touch(utcNow);
        return Result.Success();
    }

    public Result EnsureCanBeDeleted() =>
        Status == LotStatus.Draft
            ? Result.Success()
            : Result.Failure(LotErrors.DeleteAllowedOnlyInDraft);

    public bool CanBeManagedBy(Guid userId) => SellerId == userId;

    private void Touch(DateTime utcNow)
    {
        UpdatedAt = utcNow;
        Version += 1;
    }

    private static Result ValidateDraftData(
        string title,
        string? description,
        decimal startingPrice,
        decimal minBidStep,
        DateTime startsAt,
        DateTime endsAt)
    {
        if (string.IsNullOrWhiteSpace(title) || title.Trim().Length is < 3 or > 255)
            return Result.Failure(LotErrors.InvalidTitle);

        if (description is { Length: > 5000 })
            return Result.Failure(LotErrors.InvalidDescription);

        if (startingPrice <= 0)
            return Result.Failure(LotErrors.InvalidStartingPrice);

        if (minBidStep <= 0)
            return Result.Failure(LotErrors.InvalidMinBidStep);

        if (startsAt >= endsAt)
            return Result.Failure(LotErrors.InvalidSchedule);

        return Result.Success();
    }
}