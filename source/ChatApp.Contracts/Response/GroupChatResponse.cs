namespace ChatApp.Contracts.Response;

public record GroupChatResponse(Guid Id, string Name, DateTime CreatedAt, Guid CreatedById, List<UserResponse> Members, List<MessageResponse> Messages);