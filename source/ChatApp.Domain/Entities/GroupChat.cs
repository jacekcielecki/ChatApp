namespace ChatApp.Domain.Entities;

public class GroupChat
{
    public required Guid Id { get; set; }
    public required string Name { get; set; }
    public required DateTime CreatedAt { get; set; }
    public required Guid CreatedById { get; set; }
    public required List<User> Members { get; set; }
}
