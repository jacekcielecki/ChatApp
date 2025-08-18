namespace ChatApp.Shared.Model.Messages;

public record CreatePrivateChatMessageRequest(Guid ChatId, string Content);