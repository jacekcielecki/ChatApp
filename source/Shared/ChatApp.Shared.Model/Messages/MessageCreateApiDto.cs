namespace ChatApp.Shared.Model.Messages;

public class MessageCreateApiDto
{
    public Guid ChatId { get; set; }
    public required string Content { get; set; }
}
