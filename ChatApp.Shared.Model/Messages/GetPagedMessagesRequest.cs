namespace ChatApp.Shared.Model.Messages;

public record GetPagedMessagesRequest(Guid ChatId, uint PageSize, uint PageNumber);