using ChatApp.Messages.Core.Hubs;
using Microsoft.AspNetCore.SignalR;

namespace ChatApp.Messages.Commands;

public class RunSignalRHealthCheck
{
    private readonly IHubContext<MessageHub, IMessageClient> _hubContext;

    public RunSignalRHealthCheck(IHubContext<MessageHub, IMessageClient> hubContext)
    {
        _hubContext = hubContext;
    }

    public async Task Run(string message)
    {
        await _hubContext.Clients.All.ReceiveHealthCheck(message);
    }
}
