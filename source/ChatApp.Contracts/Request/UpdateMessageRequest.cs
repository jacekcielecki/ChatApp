namespace ChatApp.Contracts.Request;

public record UpdateMessageRequest(Guid Id, string Content);