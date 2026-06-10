using AutoFixture;
using Domain.Bids;
using Domain.Lots;
using Domain.UnitTests.Builders;
using Domain.UnitTests.Fixtures;
using Xunit;

namespace Domain.UnitTests.Lots;

public sealed class LotTests(DomainTestClock clock) : IClassFixture<DomainTestClock>
{
    private readonly Fixture _fixture = CreateFixture(clock);

    [Fact]
    public void Create_WithValidData_ShouldCreateDraftLot()
    {
        // Arrange
        var fixture = _fixture;

        // Act
        var lot = fixture.Create<Lot>();

        // Assert
        Assert.Equal(LotStatus.Draft, lot.Status);
        Assert.Equal(100m, lot.StartingPrice);
        Assert.Equal(lot.StartingPrice, lot.CurrentPrice);
        Assert.Null(lot.CurrentWinnerId);
        Assert.Equal(1, lot.Version);
        Assert.Equal(clock.UtcNow, lot.CreatedAt);
        Assert.Equal(clock.UtcNow, lot.UpdatedAt);
    }

    [Fact]
    public void Create_WithBlankTitle_ShouldReturnValidationFailure()
    {
        // Arrange
        var startsAt = clock.UtcNow.AddHours(1);
        var endsAt = clock.UtcNow.AddDays(1);

        // Act
        var result = Lot.Create(
            Guid.NewGuid(),
            Guid.NewGuid(),
            " ",
            "Description",
            100m,
            10m,
            startsAt,
            endsAt,
            clock.UtcNow);

        // Assert
        Assert.True(result.IsFailure);
        Assert.Equal(LotErrors.InvalidTitle, result.Error);
    }

    [Fact]
    public void Start_FromDraft_ShouldActivateLotAndRaiseDomainEvent()
    {
        // Arrange
        var lot = new LotSpecimenBuilder()
            .CreatedAt(clock.UtcNow)
            .Build();

        // Act
        var result = lot.Start(clock.UtcNow);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(LotStatus.Active, lot.Status);
        Assert.Equal(2, lot.Version);
        Assert.Contains(lot.DomainEvents, domainEvent => domainEvent is LotStartedDomainEvent);
    }

    [Fact]
    public void PlaceBid_WithAmountMeetingMinimum_ShouldUpdateAuctionStateAndRaiseDomainEvent()
    {
        // Arrange
        var bidderId = _fixture.Create<Guid>();
        var bidId = _fixture.Create<Guid>();
        var lot = new LotSpecimenBuilder()
            .CreatedAt(clock.UtcNow)
            .Active()
            .Build();

        lot.ClearDomainEvents();

        // Act
        var result = lot.PlaceBid(bidId, bidderId, 110m, clock.UtcNow);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(bidId, result.Value.Id);
        Assert.Equal(bidderId, result.Value.BidderId);
        Assert.Equal(110m, result.Value.Amount);
        Assert.Equal(110m, lot.CurrentPrice);
        Assert.Equal(bidderId, lot.CurrentWinnerId);
        Assert.Equal(3, lot.Version);
        Assert.Contains(lot.DomainEvents, domainEvent => domainEvent is BidPlacedDomainEvent);
    }

    [Fact]
    public void PlaceBid_WhenBidderIsSeller_ShouldReturnFailure()
    {
        // Arrange
        var sellerId = _fixture.Create<Guid>();
        var lot = new LotSpecimenBuilder()
            .WithSellerId(sellerId)
            .CreatedAt(clock.UtcNow)
            .Active()
            .Build();

        // Act
        var result = lot.PlaceBid(Guid.NewGuid(), sellerId, 110m, clock.UtcNow);

        // Assert
        Assert.True(result.IsFailure);
        Assert.Equal(BidErrors.SellerCannotBid, result.Error);
    }

    private static Fixture CreateFixture(DomainTestClock clock)
    {
        var fixture = new Fixture();
        fixture.Customizations.Add(new LotSpecimenBuilder().CreatedAt(clock.UtcNow));

        return fixture;
    }
}