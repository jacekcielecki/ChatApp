using ChatApp.Shared.Data.Adapters.Entities;
using ChatApp.Shared.Model.Messages;

namespace ChatApp.Messages.Core.Details;

public static class MessageExtensions
{
    public static MessageDto ToDto(this Message entity)
    {
        var dto = new MessageDto
        {
            Id = entity.Id,
            Content = entity.Content,
            ChatId = entity.ChatId,
            CreatedAt = entity.CreatedAt,
            CreatedById = entity.CreatedById
        };

        return dto;
    }
}
