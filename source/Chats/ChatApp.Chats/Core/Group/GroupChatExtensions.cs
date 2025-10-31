using ChatApp.Shared.Data.Adapters.Entities;
using ChatApp.Shared.Model.Chats;
using ChatApp.Shared.Model.Messages;
using ChatApp.Shared.Model.Users;

namespace ChatApp.Chats.Core.Group;

public static class GroupChatExtensions
{
    public static ChatDto ToDto(this GroupChat entity)
    {
        var members = entity.Members
            .Select(member => new UserDto
            {
                Id = member.Id,
                Email = member.Email,
                GivenName = member.GivenName,
                FamilyName = member.FamilyName,
                ProfilePictureUrl = member.ProfilePictureUrl,
                Bio = member.Bio,
                CreatedAt = member.CreatedAt
            })
            .ToList();

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
            Type = ChatType.Group,
            Name = entity.Name,
            CreatedAt = entity.CreatedAt,
            CreatedById = entity.CreatedById,
            Members = members,
            Messages = messages
        };

        return dto;
    }
}
