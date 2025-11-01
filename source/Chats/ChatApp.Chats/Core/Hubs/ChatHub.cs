using ChatApp.Shared.Model.Chats;
using ChatApp.Shared.Model.Messages;
using Microsoft.AspNetCore.SignalR;

namespace ChatApp.Chats.Core.Hubs;

public interface IMessageClient
{
    Task ReceiveHealthCheck(string message);
    Task ReceiveMessage(MessageDto message);
    Task ReceiveMessageUpdate(MessageDto message);
    Task ReceiveMessageDelete(Guid chatId, Guid messageId);
    Task ReceiveChat(ChatDto chat);
    Task ReceiveChatUpdate(ChatDto chat);
}

public sealed class ChatHub : Hub<IMessageClient>
{
    public async Task JoinChats(List<Guid> chats)
    {
        var isAuthenticated = Context.User.Identity?.IsAuthenticated is true;
        if (isAuthenticated)
        {
            foreach (var chat in chats)
            {
                await Groups.AddToGroupAsync(Context.ConnectionId, $"Chat:{chat}");
            }
        }
    }

    public async Task LeaveChats(List<Guid> chats)
    {
        foreach (var chat in chats)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, $"Chat:{chat}");
        }
    }
}
