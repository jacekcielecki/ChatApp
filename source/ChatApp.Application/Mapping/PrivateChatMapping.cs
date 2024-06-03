using ChatApp.Contracts.Response;
using ChatApp.Domain.Entities;

namespace ChatApp.Application.Mapping;

public static class PrivateChatMapping
{
    public static PrivateChatResponse ToPrivateChatResponse(this PrivateChat privateChat)
    {
        var messagesResponse = privateChat.Messages.Select(m => new MessageResponse(m.Id, m.Content, m.ChatId, m.CreatedAt, m.CreatedById)).ToList();

        return new PrivateChatResponse(privateChat.Id, privateChat.CreatedAt, privateChat.Receiver?.ToUserResponse(), messagesResponse);
    }

    public static List<PrivateChatResponse> ToPrivateChatResponse(this IEnumerable<PrivateChat> privateChat)
    {
        return privateChat.Select(pc => pc.ToPrivateChatResponse()).ToList();
    }
}
