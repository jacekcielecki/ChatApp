namespace ChatApp.Shared.Model.Chats;

public record GetChatResponse(
    List<PrivateChatResponse> PrivateChats,
    List<GroupChatResponse> GroupChats
    );