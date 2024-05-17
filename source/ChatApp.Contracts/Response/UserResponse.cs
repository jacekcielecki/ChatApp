namespace ChatApp.Contracts.Response;

public record UserResponse(Guid Id, string Email, DateTime CreatedAt);