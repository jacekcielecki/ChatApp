using ChatApp.Messages.Core.Details;
using ChatApp.Messages.Core.Hubs;
using ChatApp.Shared.Model.Messages;
using ChatApp.Shared.Model.ValueObjects;
using Microsoft.AspNetCore.SignalR;
using OneOf;
using OneOf.Types;

namespace ChatApp.Messages.Commands;

public class UpdateMessage
{
    private readonly GetMessageByIdRepository _getMessageByIdRepository;
    private readonly UpdateMessageRepository _updateMessageRepository;
    private readonly IHubContext<MessageHub, IMessageClient> _messageClient;

    public UpdateMessage(
        GetMessageByIdRepository getMessageByIdRepository,
        UpdateMessageRepository updateMessageRepository,
        IHubContext<MessageHub, IMessageClient> messageClient
        )
    {
        _getMessageByIdRepository = getMessageByIdRepository;
        _updateMessageRepository = updateMessageRepository;
        _messageClient = messageClient;
    }

    public async Task<OneOf<Success, NotFound, Forbidden, ValidationErrors>> Update(MessageUpdateApiDto dto, Guid userId)
    {
        var message = await _getMessageByIdRepository.Get(dto.Id);
        if (message is null)
        {
            return new NotFound();
        }

        if (message.CreatedById != userId)
        {
            return new Forbidden();
        }

        var validationErrors = ValidateRequest(dto);
        if (validationErrors.Any())
        {
            return new ValidationErrors(validationErrors);
        }

        message.Content = dto.Content;

        await _updateMessageRepository.Update(message);

        await _messageClient.Clients.Group($"Chat:{message.ChatId}").ReceiveMessageDelete(message.ChatId, message.Id);

        return new Success();
    }

    public Dictionary<string, string[]> ValidateRequest(MessageUpdateApiDto request)
    {
        var validationErrors = new Dictionary<string, string[]>();

        const int maxContentLength = 2000;
        if (request.Content.Length > maxContentLength)
        {
            validationErrors.Add(nameof(MessageUpdateApiDto.Content), ["Message content maximum length is 2000 characters"]);
        }

        return validationErrors;
    }
}
