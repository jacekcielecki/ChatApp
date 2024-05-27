using ChatApp.Contracts.Response;
using ChatApp.Domain.Entities;

namespace ChatApp.Application.Mapping;

public static class GroupChatMapping
{
    public static GroupChatResponse ToGroupChatResponse(this GroupChat groupChat)
    {
        var membersResponse = groupChat.Members.Select(m => new UserResponse(m.Id, m.Email, m.CreatedAt)).ToList();
        var messagesResponse = groupChat.Messages.Select(m => new MessageResponse(m.Id, m.Content, m.CreatedAt, m.CreatedById)).ToList();

        return new GroupChatResponse(groupChat.Id, groupChat.Name, groupChat.CreatedAt, groupChat.CreatedById, membersResponse, messagesResponse);
    }

    public static List<GroupChatResponse> ToGroupChatResponse(this IEnumerable<GroupChat> groupChats)
    {
        return groupChats.Select(gc => gc.ToGroupChatResponse()).ToList();
    }
}
