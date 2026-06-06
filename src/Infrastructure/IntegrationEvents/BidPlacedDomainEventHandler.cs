using Domain.Bids;
using Infrastructure.Realtime;
using MediatR;
using Microsoft.AspNetCore.SignalR;

namespace Infrastructure.IntegrationEvents;

internal sealed class BidPlacedDomainEventHandler(
    IHubContext<LotEventsHub> hubContext)
    : INotificationHandler<BidPlacedDomainEvent>
{
    public async Task Handle(BidPlacedDomainEvent domainEvent, CancellationToken cancellationToken)
    {
        var eventId = Guid.NewGuid();
        var occurredOnUtc = DateTime.UtcNow;

        await hubContext.Clients.All.SendAsync(
            LotEventsHub.BidPlacedMethod,
            new
            {
                id = eventId,
                bidId = domainEvent.BidId,
                lotId = domainEvent.LotId,
                bidderId = domainEvent.BidderId,
                amount = domainEvent.Amount,
                currentPrice = domainEvent.CurrentPrice,
                currentWinnerId = domainEvent.CurrentWinnerId,
                lotVersion = domainEvent.LotVersion,
                bidCreatedAt = domainEvent.BidCreatedAt,
                lotUpdatedAt = domainEvent.LotUpdatedAt,
                occurredOnUtc
            },
            cancellationToken);
    }
}