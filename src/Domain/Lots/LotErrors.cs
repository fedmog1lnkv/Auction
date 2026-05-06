using SharedKernel;

namespace Domain.Lots;

public static class LotErrors
{
    public static readonly Error NotFound = Error.NotFound(
        "Lots.NotFound",
        "The lot was not found.");

    public static readonly Error Forbidden = new(
        "Lots.Forbidden",
        "You are not allowed to manage this lot.",
        ErrorType.Forbidden);

    public static readonly Error InvalidTitle = new(
        "Lots.InvalidTitle",
        "Title must be between 3 and 255 characters.",
        ErrorType.Validation);

    public static readonly Error InvalidDescription = new(
        "Lots.InvalidDescription",
        "Description must be at most 5000 characters.",
        ErrorType.Validation);

    public static readonly Error InvalidStartingPrice = new(
        "Lots.InvalidStartingPrice",
        "Starting price must be greater than 0.",
        ErrorType.Validation);

    public static readonly Error InvalidMinBidStep = new(
        "Lots.InvalidMinBidStep",
        "Minimum bid step must be greater than 0.",
        ErrorType.Validation);

    public static readonly Error InvalidSchedule = new(
        "Lots.InvalidSchedule",
        "Lot schedule is invalid. startsAt must be before endsAt.",
        ErrorType.Validation);

    public static readonly Error EndsAtMustBeInFuture = new(
        "Lots.EndsAtMustBeInFuture",
        "Lot end time must be in the future.",
        ErrorType.Validation);

    public static readonly Error UpdateAllowedOnlyInDraft = Error.Conflict(
        "Lots.UpdateAllowedOnlyInDraft",
        "Lot can be updated only in DRAFT status.");

    public static readonly Error DeleteAllowedOnlyInDraft = Error.Conflict(
        "Lots.DeleteAllowedOnlyInDraft",
        "Only DRAFT lots can be deleted.");

    public static Error InvalidStatusTransition(LotStatus from, LotStatus to) =>
        Error.Conflict(
            "Lots.InvalidStatusTransition",
            $"Invalid lot status transition: {from.ToString().ToUpperInvariant()} -> {to.ToString().ToUpperInvariant()}");
}