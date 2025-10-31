using ChatApp.Shared.Data.Adapters.Entities;
using ChatApp.Shared.Model.Chats;
using ChatApp.Shared.Model.Messages;
using ChatApp.Shared.Model.Users;

namespace ChatApp.Chats.Core.Private;

public static class PrivateChatExtensions
{
    public static ChatDto ToDto(this PrivateChat entity)
    {
        if (entity.Receiver is null)
            throw new NullReferenceException("Unable to map to dto, PrivateChat Receiver is null");

        var receiverDto = new UserDto
        {
            Id = entity.Receiver.Id,
            Email = entity.Receiver.Email,
            GivenName = entity.Receiver.GivenName,
            FamilyName = entity.Receiver.FamilyName,
            ProfilePictureUrl = entity.Receiver.ProfilePictureUrl,
            Bio = entity.Receiver.Bio,
            CreatedAt = entity.Receiver.CreatedAt
        };

        var messages = entity.Messages
            .Select(msg => new MessageDto
            {
                Id = msg.Id,
                Content = msg.Content,
                ChatId = msg.ChatId,
                CreatedAt = msg.CreatedAt,
                CreatedById = msg.CreatedById
            })
            .OrderByDescending(x => x.CreatedAt)
            .ToList();

        var dto = new ChatDto
        {
            Id = entity.Id,
            Type = ChatType.Private,
            Name =  $"{entity.Receiver.GivenName} {entity.Receiver.FamilyName}",
            CreatedAt = entity.CreatedAt,
            CreatedById = entity.FirstUserId,
            Members = [receiverDto],
            Messages = messages
        };

        return dto;
    }
}
