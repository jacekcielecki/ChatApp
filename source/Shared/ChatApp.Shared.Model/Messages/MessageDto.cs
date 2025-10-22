namespace ChatApp.Shared.Model.Messages;

public class MessageDto
{
    public Guid Id { get; set; }
    public required string Content { get; set; }
    public Guid ChatId { get; set; }
    public DateTime CreatedAt { get; set; }
    public Guid CreatedById { get; set; }
}