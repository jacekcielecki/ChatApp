namespace ChatApp.Contracts.Request;

public record CreateGroupMessageRequest(Guid ChatId, string Content);
