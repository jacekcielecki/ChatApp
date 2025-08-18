namespace ChatApp.Shared.Data.Adapters.Entities;

public class User
{
    public required Guid Id { get; set; }
    public required string Email { get; set; }
    public required DateTime CreatedAt { get; set; }
}