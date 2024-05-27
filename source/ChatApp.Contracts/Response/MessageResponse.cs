namespace ChatApp.Contracts.Response;

public record MessageResponse(Guid Id, string Content, DateTime CreatedAt, Guid CreatedById);