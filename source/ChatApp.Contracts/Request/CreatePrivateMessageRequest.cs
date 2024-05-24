namespace ChatApp.Contracts.Request;

public record CreatePrivateMessageRequest(string ReceiverEmail, string Content);
