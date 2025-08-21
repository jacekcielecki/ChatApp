using ChatApp.Shared.Model.Messages;
using ChatApp.Shared.Model.Users;

namespace ChatApp.Shared.Model.Chats;

public record GroupChatResponse(
    Guid Id,
    string Name,
    DateTime CreatedAt,
    Guid CreatedById,
    List<UserResponse> Members,
    List<MessageResponse> Messages
    );