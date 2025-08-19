using ChatApp.Shared.Data.Adapters.Entities;
using ChatApp.Shared.Model.Messages;

namespace ChatApp.Messages.Core;

public static class MessageMapping
{
    public static MessageResponse ToResponse(this Message message)
    {
        return new MessageResponse(
            message.Id,
            message.Content,
            message.ChatId,
            message.CreatedAt,
            message.CreatedById);
    }
}
