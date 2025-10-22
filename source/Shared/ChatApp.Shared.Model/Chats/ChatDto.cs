using ChatApp.Shared.Model.Messages;
using ChatApp.Shared.Model.Users;

namespace ChatApp.Shared.Model.Chats;

public class ChatDto
{
    public Guid Id { get; set; }
    public ChatType Type { get; set; }
    public required string Name { get; set; }
    public DateTime CreatedAt { get; set; }
    public Guid CreatedById { get; set; }
    public List<UserDto> Members { get; set; } = [];
    public List<MessageDto> Messages { get; set; } = [];
}