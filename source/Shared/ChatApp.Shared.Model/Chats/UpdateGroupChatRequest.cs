namespace ChatApp.Shared.Model.Chats;

public record UpdateGroupChatRequest(Guid Id, string Name, Guid[] Members);