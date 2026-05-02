using Microsoft.Extensions.DependencyInjection;
using SharedKernel;

namespace Infrastructure.DomainEvents;

internal sealed class DomainEventsDispatcher(IServiceProvider serviceProvider) : IDomainEventsDispatcher
{
    public async Task DispatchAsync(
        IReadOnlyCollection<IDomainEvent> domainEvents,
        CancellationToken cancellationToken = default)
    {
        foreach (IDomainEvent domainEvent in domainEvents)
        {
            Type handlerType = typeof(IDomainEventHandler<>).MakeGenericType(domainEvent.GetType());
            var handlers = serviceProvider.GetServices(handlerType);

            foreach (var handler in handlers)
            {
                await ((dynamic)handler).Handle((dynamic)domainEvent, cancellationToken);
            }
        }
    }
}