namespace ChatApp.Contracts.Response;

public record PrivateChatResponse(Guid Id, DateTime CreatedAt, Guid FirstUserId, Guid SecondUserId);