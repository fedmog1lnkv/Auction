using Domain.Lots;
using Infrastructure.Realtime;
using MediatR;
using Microsoft.AspNetCore.SignalR;

namespace Infrastructure.IntegrationEvents;

internal sealed class LotStartedDomainEventHandler(
    IHubContext<LotEventsHub> hubContext)
    : INotificationHandler<LotStartedDomainEvent>
{
    public async Task Handle(LotStartedDomainEvent domainEvent, CancellationToken cancellationToken)
    {
        var eventId = Guid.NewGuid();
        var occurredOnUtc = DateTime.UtcNow;

        await hubContext.Clients.All.SendAsync(
            LotEventsHub.LotStartedMethod,
            new
            {
                id = eventId,
                lotId = domainEvent.LotId,
                title = domainEvent.Title,
                description = domainEvent.Description,
                sellerId = domainEvent.SellerId,
                startingPrice = domainEvent.StartingPrice,
                minBidStep = domainEvent.MinBidStep,
                currentPrice = domainEvent.CurrentPrice,
                currentWinnerId = domainEvent.CurrentWinnerId,
                status = domainEvent.Status.ToString().ToUpperInvariant(),
                startsAt = domainEvent.StartsAt,
                endsAt = domainEvent.EndsAt,
                version = domainEvent.Version,
                lotCreatedAt = domainEvent.CreatedAt,
                lotUpdatedAt = domainEvent.UpdatedAt,
                occurredOnUtc = occurredOnUtc
            },
            cancellationToken);
    }
}