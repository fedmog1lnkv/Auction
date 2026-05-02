using Domain.Users;
using Infrastructure.Outbox;
using Microsoft.EntityFrameworkCore;
using SharedKernel;
using System.Text.Json;

namespace Infrastructure.Persistence;

public sealed class ApplicationDbContext(
    DbContextOptions<ApplicationDbContext> options) : DbContext(options)
{
    public DbSet<User> Users => Set<User>();
    public DbSet<RefreshToken> RefreshTokens => Set<RefreshToken>();
    public DbSet<OutboxMessage> OutboxMessages => Set<OutboxMessage>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);
        base.OnModelCreating(modelBuilder);
    }

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        AddDomainEventsToOutboxMessages();
        return await base.SaveChangesAsync(cancellationToken);
    }

    private void AddDomainEventsToOutboxMessages()
    {
        List<IDomainEvent> domainEvents = ChangeTracker
            .Entries<Entity>()
            .Select(entry => entry.Entity)
            .SelectMany(entity =>
            {
                List<IDomainEvent> events = entity.DomainEvents;
                entity.ClearDomainEvents();
                return events;
            })
            .ToList();

        if (domainEvents.Count == 0)
        {
            return;
        }

        var outboxMessages = domainEvents.Select(domainEvent => new OutboxMessage
        {
            Id = Guid.NewGuid(),
            OccurredOnUtc = DateTime.UtcNow,
            Type = domainEvent.GetType().AssemblyQualifiedName ?? domainEvent.GetType().FullName ?? domainEvent.GetType().Name,
            Content = JsonSerializer.Serialize(domainEvent, domainEvent.GetType())
        });

        OutboxMessages.AddRange(outboxMessages);
    }
}
