namespace ChatApp.Shared.Model.Users;

public class UserSummaryDto
{
    public Guid Id { get; set; }
    public required string Email { get; set; }
    public string? GivenName { get; set; }
    public string? FamilyName { get; set; }
}