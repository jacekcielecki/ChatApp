namespace ChatApp.Shared.Data.Adapters.Entities;

public class User
{
    public Guid Id { get; set; }
    public string? Email { get; set; }
    public DateTime CreatedAt { get; set; }
}