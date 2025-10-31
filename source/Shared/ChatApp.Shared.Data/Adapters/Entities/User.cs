namespace ChatApp.Shared.Data.Adapters.Entities;

public class User
{
    public Guid Id { get; set; }
    public required string Email { get; set; }
    public required string GivenName { get; set; }
    public required string FamilyName { get; set; }
    public string? ProfilePictureUrl { get; set; }
    public string? Bio { get; set; }
    public DateTime CreatedAt { get; set; }
}