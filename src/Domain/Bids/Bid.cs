using SharedKernel;

namespace Domain.Bids;

public sealed class Bid : Entity
{
    private Bid()
    {
    }

    public Guid Id { get; private set; }
    public Guid LotId { get; private set; }
    public Guid BidderId { get; private set; }
    public decimal Amount { get; private set; }
    public DateTime CreatedAt { get; private set; }

    internal static Result<Bid> Create(
        Guid id,
        Guid lotId,
        Guid bidderId,
        decimal amount,
        DateTime utcNow)
    {
        if (amount <= 0)
            return Result.Failure<Bid>(BidErrors.InvalidAmount);

        var bid = new Bid
        {
            Id = id,
            LotId = lotId,
            BidderId = bidderId,
            Amount = amount,
            CreatedAt = utcNow
        };

        return Result.Success(bid);
    }
}