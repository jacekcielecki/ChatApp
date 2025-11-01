using ChatApp.Chats.Core.Hubs;
using Microsoft.AspNetCore.SignalR;

namespace ChatApp.Messages.Commands;

public class RunSignalRHealthCheck
{
    private readonly IHubContext<ChatHub, IMessageClient> _hubContext;

    public RunSignalRHealthCheck(IHubContext<ChatHub, IMessageClient> hubContext)
    {
        _hubContext = hubContext;
    }

    public async Task Run(string message)
    {
        await _hubContext.Clients.All.ReceiveHealthCheck(message);
    }
}
