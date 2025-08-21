namespace ChatApp.Shared.Model.Users;

public record UserResponse(
    Guid Id,
    string Email,
    DateTime CreatedAt
    );