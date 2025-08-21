namespace ChatApp.Shared.Model.Chats;

public record CreateGroupChatRequest(string Name, Guid[] Members);