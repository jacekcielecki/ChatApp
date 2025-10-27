namespace ChatApp.Shared.Model.Chats;

public class ContactsAndChatsDto
{
    public List<ChatDto> Chats { get; set; } = [];
    public Dictionary<Guid, string> Contacts { get; set; } = []; // Key: UserId, Value: UserName
}
