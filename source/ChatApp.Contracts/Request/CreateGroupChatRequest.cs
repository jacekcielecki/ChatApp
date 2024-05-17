namespace ChatApp.Contracts.Request;

public record CreateGroupChatRequest(string Name, Guid[] Members);

