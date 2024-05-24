namespace ChatApp.Contracts.Response;

public record GetChatResponse(List<PrivateChatResponse> PrivateChats, List<GroupChatResponse> GroupChats);