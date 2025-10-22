namespace ChatApp.Shared.Model.Chats;

public record GroupChatUpdateApiDto(Guid Id, string Name, Guid[] Members);