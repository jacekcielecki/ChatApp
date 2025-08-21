namespace ChatApp.Shared.Model.Messages;

public record MessageResponse(
    Guid Id,
    string Content,
    Guid ChatId,
    DateTime CreatedAt,
    Guid CreatedById
    );