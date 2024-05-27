namespace ChatApp.Contracts.Response;

public record PrivateChatResponse(Guid Id, DateTime CreatedAt, UserResponse? Receiver, List<MessageResponse> Messages);