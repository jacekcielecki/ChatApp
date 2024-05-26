namespace ChatApp.Contracts.Request;

public record CreatePrivateMessageRequest(Guid ChatId, string Content);
