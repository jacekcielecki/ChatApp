namespace ChatApp.Shared.Model.Chats;

public record GroupChatCreateApiDto(string Name, Guid[] Members);