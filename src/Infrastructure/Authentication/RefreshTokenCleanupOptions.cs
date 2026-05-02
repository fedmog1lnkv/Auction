namespace Infrastructure.Authentication;

public sealed class RefreshTokenCleanupOptions
{
    public int IntervalHours { get; set; } = 24;
    public int RevokedRetentionDays { get; set; } = 30;
    public int ExpiredRetentionDays { get; set; } = 7;
}