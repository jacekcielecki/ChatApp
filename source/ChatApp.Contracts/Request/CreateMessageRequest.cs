namespace ChatApp.Contracts.Request;

public record CreateMessageRequest(Guid ChatId, string Content);
