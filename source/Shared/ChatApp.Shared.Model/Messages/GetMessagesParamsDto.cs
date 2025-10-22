using ChatApp.Shared.Model.Chats;

namespace ChatApp.Shared.Model.Messages;

public record GetMessagesParamsDto(
    Guid ChatId,
    ChatType ChatType,
    int PageSize, 
    int PageNumber
);