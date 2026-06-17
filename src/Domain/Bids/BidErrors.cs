using SharedKernel;

namespace Domain.Bids;

public static class BidErrors
{
    public static readonly Error InvalidAmount = new(
        "Bids.InvalidAmount",
        "Bid amount must be greater than 0.",
        ErrorType.Validation);

    public static readonly Error SellerCannotBid = new(
        "Bids.SellerCannotBid",
        "Seller cannot place bids on their own lot.",
        ErrorType.Forbidden);

    public static readonly Error LotNotActive = Error.Conflict(
        "Bids.LotNotActive",
        "Bids can be placed only on active lots.");

    public static readonly Error LotEnded = Error.Conflict(
        "Bids.LotEnded",
        "Bids cannot be placed after the lot has ended.");

    public static Error AmountTooLow(decimal minimumAmount) =>
        Error.Conflict(
            "Bids.AmountTooLow",
            $"Bid amount must be at least {minimumAmount:0.00}.");

    public static readonly Error ConcurrentChange = Error.Conflict(
        "Bids.ConcurrentChange",
        "Bid could not be placed because the lot was updated concurrently. Please try again.");
}