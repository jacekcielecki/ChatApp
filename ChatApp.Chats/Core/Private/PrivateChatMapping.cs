using ChatApp.Shared.Data.Adapters.Entities;
using ChatApp.Shared.Model.Chats;
using ChatApp.Shared.Model.Messages;
using ChatApp.Shared.Model.Users;

namespace ChatApp.Chats.Core.Private;

public static class PrivateChatMapping
{
    public static PrivateChatResponse ToResponse(this PrivateChat chat)
    {
       var receiver = new UserResponse(
           chat.Receiver.Id,
           chat.Receiver.Email,
           chat.Receiver.CreatedAt);
       
       var response = new PrivateChatResponse(
           chat.Id, 
           chat.CreatedAt,
           receiver,
           chat.Messages
               .Select(m => new MessageResponse(m.Id, m.Content, m.ChatId, m.CreatedAt, m.CreatedById))
               .ToList());

        return response;
    }

    public static List<PrivateChatResponse> ToResponse(this IEnumerable<PrivateChat> privateChat)
    {
        var chats = privateChat
            .Select(pc => pc.ToResponse())
            .ToList();

        return chats;
    }
}
