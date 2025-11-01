using Microsoft.AspNetCore.SignalR;

namespace ChatApp.Messages.Core.Hubs;

public sealed class MessageHub : Hub
{
    public async Task SendHealthCheck(string message)
    {
        await Clients.All.SendAsync("ReceiveHealthCheck", message);
    }
}
