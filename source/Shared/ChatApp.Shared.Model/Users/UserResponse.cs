namespace ChatApp.Shared.Model.Users;

public record UserResponse(
    Guid Id,
    string? Email,
    string? GivenName,
    string? FamilyName,
    DateTime CreatedAt
    );