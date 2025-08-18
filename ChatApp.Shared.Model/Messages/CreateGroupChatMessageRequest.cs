namespace ChatApp.Shared.Model.Messages;

public record CreateGroupChatMessageRequest(Guid ChatId, string Content);
