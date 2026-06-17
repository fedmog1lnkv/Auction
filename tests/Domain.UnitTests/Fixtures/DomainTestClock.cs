namespace Domain.UnitTests.Fixtures;

public sealed class DomainTestClock
{
    public DateTime UtcNow { get; } = new(2026, 06, 10, 12, 00, 00, DateTimeKind.Utc);
}