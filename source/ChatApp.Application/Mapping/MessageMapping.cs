using ChatApp.Contracts.Response;
using ChatApp.Domain.Entities;

namespace ChatApp.Application.Mapping;

public static class MessageMapping
{
    public static MessageResponse ToMessageResponse(this Message message)
    {
        return new MessageResponse(message.Id, message.Content, message.ChatId, message.CreatedAt, message.CreatedById);
    }

    public static List<MessageResponse> ToMessageResponse(this IEnumerable<Message> messages)
    {
        return messages.Select(gc => gc.ToMessageResponse()).ToList();
    }
}
