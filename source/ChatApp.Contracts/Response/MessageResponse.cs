namespace ChatApp.Contracts.Response;

public record MessageResponse(Guid Id, string Content, Guid ChatId, DateTime CreatedAt, Guid CreatedById);