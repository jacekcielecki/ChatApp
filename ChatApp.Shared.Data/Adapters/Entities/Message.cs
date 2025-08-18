namespace ChatApp.Shared.Data.Adapters.Entities;

public class Message
{
    public required Guid Id { get; set; }
    public required string Content { get; set; }
    public required Guid ChatId { get; set; }
    public required DateTime CreatedAt { get; set; }
    public required Guid CreatedById { get; set; }
}
