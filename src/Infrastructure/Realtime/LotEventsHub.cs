using Microsoft.AspNetCore.SignalR;

namespace Infrastructure.Realtime;

public sealed class LotEventsHub : Hub
{
    public const string HubRoute = "/hubs/lots";
    public const string LotStartedMethod = "lotStarted";
}