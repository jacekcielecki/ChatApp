namespace ChatApp.Domain.Entities;

public class PrivateChat
{
    public required Guid Id { get; set; }
    public required DateTime CreatedAt { get; set; }
    public required Guid FirstUserId { get; set; }
    public required Guid SecondUserId { get; set; }
    public User? Receiver { get; set; }
    public List<Message> Messages { get; set; } = [];
}
