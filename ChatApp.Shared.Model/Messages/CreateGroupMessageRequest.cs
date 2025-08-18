namespace ChatApp.Shared.Model.Messages;

public record CreateGroupMessageRequest(Guid ChatId, string Content);
