using AutoFixture.Kernel;
using Domain.Lots;

namespace Domain.UnitTests.Builders;

public sealed class LotSpecimenBuilder : ISpecimenBuilder
{
    private Guid _id = Guid.Parse("10000000-0000-0000-0000-000000000001");
    private Guid _sellerId = Guid.Parse("20000000-0000-0000-0000-000000000001");
    private string _title = "Vintage camera";
    private string? _description = "Working condition, includes original leather case.";
    private decimal _startingPrice = 100m;
    private decimal _minBidStep = 10m;
    private DateTime _startsAt = new(2026, 06, 10, 13, 00, 00, DateTimeKind.Utc);
    private DateTime _endsAt = new(2026, 06, 11, 13, 00, 00, DateTimeKind.Utc);
    private DateTime _utcNow = new(2026, 06, 10, 12, 00, 00, DateTimeKind.Utc);
    private bool _active;

    public object Create(object request, ISpecimenContext context)
    {
        if (request is Type type && type == typeof(Lot))
            return Build();

        return new NoSpecimen();
    }

    public LotSpecimenBuilder WithId(Guid id)
    {
        _id = id;
        return this;
    }

    public LotSpecimenBuilder WithSellerId(Guid sellerId)
    {
        _sellerId = sellerId;
        return this;
    }

    public LotSpecimenBuilder WithTitle(string title)
    {
        _title = title;
        return this;
    }

    public LotSpecimenBuilder WithSchedule(DateTime startsAt, DateTime endsAt)
    {
        _startsAt = startsAt;
        _endsAt = endsAt;
        return this;
    }

    public LotSpecimenBuilder WithPrices(decimal startingPrice, decimal minBidStep)
    {
        _startingPrice = startingPrice;
        _minBidStep = minBidStep;
        return this;
    }

    public LotSpecimenBuilder CreatedAt(DateTime utcNow)
    {
        _utcNow = utcNow;
        return this;
    }

    public LotSpecimenBuilder Active()
    {
        _active = true;
        return this;
    }

    public Lot Build()
    {
        var result = Lot.Create(
            _id,
            _sellerId,
            _title,
            _description,
            _startingPrice,
            _minBidStep,
            _startsAt,
            _endsAt,
            _utcNow);

        if (result.IsFailure)
            throw new InvalidOperationException($"Invalid lot specimen: {result.Error.Code}");

        var lot = result.Value;
        if (_active)
        {
            var startResult = lot.Start(_utcNow);
            if (startResult.IsFailure)
                throw new InvalidOperationException($"Invalid active lot specimen: {startResult.Error.Code}");
        }

        return lot;
    }
}