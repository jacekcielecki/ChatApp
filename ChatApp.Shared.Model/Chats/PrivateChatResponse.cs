using ChatApp.Shared.Model.Messages;
using ChatApp.Shared.Model.Users;

namespace ChatApp.Shared.Model.Chats;

public record PrivateChatResponse(
    Guid Id,
    DateTime CreatedAt,
    UserResponse? Receiver,
    List<MessageResponse> Messages
    );