using ChatApp.Shared.Data.Adapters.Entities;
using ChatApp.Shared.Model.Chats;
using ChatApp.Shared.Model.Messages;
using ChatApp.Shared.Model.Users;

namespace ChatApp.Chats.Core.Group;

public static class GroupChatMapping
{
    public static GroupChatResponse ToResponse(this GroupChat groupChat)
    {
        var membersResponse = groupChat.Members
            .Select(m => new UserResponse(m.Id, m.Email, m.CreatedAt))
            .ToList();

        var messagesResponse = groupChat.Messages
            .Select(m => new MessageResponse(m.Id, m.Content, m.ChatId, m.CreatedAt, m.CreatedById))
            .ToList();

        return new GroupChatResponse(groupChat.Id, groupChat.Name, groupChat.CreatedAt, groupChat.CreatedById, membersResponse, messagesResponse);
    }

    public static List<GroupChatResponse> ToResponse(this IEnumerable<GroupChat> groupChats)
    {
        var chats = groupChats
            .Select(gc => gc.ToResponse())
            .ToList();

        return chats;
    }
}
