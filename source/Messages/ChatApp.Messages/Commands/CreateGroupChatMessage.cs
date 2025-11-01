using ChatApp.Chats.Core.Group;
using ChatApp.Messages.Core.Details;
using ChatApp.Messages.Core.Hubs;
using ChatApp.Shared.Data.Adapters.Entities;
using ChatApp.Shared.Model.Messages;
using ChatApp.Shared.Model.ValueObjects;
using Microsoft.AspNetCore.SignalR;
using OneOf;
using OneOf.Types;

namespace ChatApp.Messages.Commands;

public class CreateGroupChatMessage
{
    private readonly GetGroupChatByIdRepository _getGroupChatByIdRepository;
    private readonly CreateMessageRepository _createMessageRepository;
    private readonly IHubContext<MessageHub, IMessageClient> _messageClient;

    public CreateGroupChatMessage(
        GetGroupChatByIdRepository getGroupChatByIdRepository,
        CreateMessageRepository createMessageRepository,
        IHubContext<MessageHub, IMessageClient> messageClient
        )
    {
        _getGroupChatByIdRepository = getGroupChatByIdRepository;
        _createMessageRepository = createMessageRepository;
        _messageClient = messageClient;
    }

    public async Task<OneOf<Success<Guid?>, Forbidden, ValidationErrors>> Create(MessageCreateApiDto dto, Guid userId)
    {
        var validationErrors = ValidateRequest(dto);
        if (validationErrors.Any())
        {
            return new ValidationErrors(validationErrors);
        }

        var authorizationErrors = await Authorize(dto, userId);
        if (authorizationErrors.Any())
        {
            return new Forbidden(authorizationErrors);
        }

        var message = new Message
        {
            Id = Guid.NewGuid(),
            ChatId = dto.ChatId,
            Content = dto.Content,
            CreatedAt = DateTime.UtcNow,
            CreatedById = userId
        };

        var id = await _createMessageRepository.Create(message);

        await _messageClient.Clients.Group($"Chat:{message.ChatId}").ReceiveMessage(message.ToDto());

        return new Success<Guid?>(id);
    }

    private Dictionary<string, string[]> ValidateRequest(MessageCreateApiDto? request)
    {
        var validationErrors = new Dictionary<string, string[]>();

        if (request is null)
        {
            validationErrors.Add(nameof(MessageCreateApiDto), ["Request body not set"]);
            return validationErrors;
        }

        var maxContentLength = 2000;
        if (request.Content.Length > maxContentLength)
        {
            validationErrors.Add(nameof(MessageCreateApiDto.Content), ["Message content maximum length is 2000 characters"]);
        }

        return validationErrors;
    }

    private async Task<Dictionary<string, string[]>> Authorize(MessageCreateApiDto dto, Guid userId)
    {
        var errors = new Dictionary<string, string[]>();

        var chat = await _getGroupChatByIdRepository.Get(dto.ChatId);
        if (chat is null)
        {
            errors.Add(nameof(MessageCreateApiDto.ChatId), ["Group chat with specified id not found"]);
            return errors;
        }

        if (chat.Members.All(x => x.Id != userId))
        {
            errors.Add(nameof(GroupChat.Members), ["User is not allowed to send messages on specified chat"]);
            return errors;
        }

        return errors;
    }
}
