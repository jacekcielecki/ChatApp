namespace ChatApp.Contracts.Request;

public record GetPagedMessagesRequest(Guid ChatId, uint PageSize, uint PageNumber);