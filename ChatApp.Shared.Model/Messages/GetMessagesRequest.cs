using ChatApp.Shared.Model.Chats;

namespace ChatApp.Shared.Model.Messages;

public record GetMessagesRequest(
    Guid ChatId,
    ChatType ChatType,
    int PageSize, 
    int PageNumber);