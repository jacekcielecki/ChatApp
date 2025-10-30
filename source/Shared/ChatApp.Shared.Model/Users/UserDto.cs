namespace ChatApp.Shared.Model.Users;

public class UserDto
{
    public Guid Id { get; set; }
    public required string Email { get; set; }
    public required string GivenName { get; set; }
    public required string FamilyName { get; set; }
    public string? ProfilePicUrl { get; set; }
    public string? Bio { get; set; }
    public DateTime CreatedAt { get; set; }
}