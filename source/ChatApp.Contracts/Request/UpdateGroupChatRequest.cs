namespace ChatApp.Contracts.Request;

public record UpdateGroupChatRequest(Guid Id, string Name, Guid[] Members);