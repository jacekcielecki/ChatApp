using ChatApp.Chats.Core.Group;
using ChatApp.Messages.Core.Details;
using ChatApp.Shared.Data.Adapters.Entities;
using ChatApp.Shared.Model.Messages;
using ChatApp.Shared.Model.ValueObjects;
using OneOf;
using OneOf.Types;

namespace ChatApp.Messages.Commands;

public class CreateGroupChatMessage
{
    private readonly GetGroupChatByIdRepository _getGroupChatByIdRepository;
    private readonly CreateMessageRepository _createMessageRepository;

    public CreateGroupChatMessage(GetGroupChatByIdRepository getGroupChatByIdRepository, CreateMessageRepository createMessageRepository)
    {
        _getGroupChatByIdRepository = getGroupChatByIdRepository;
        _createMessageRepository = createMessageRepository;
    }

    public async Task<OneOf<Success<Guid?>, Forbidden, ValidationErrors>> Create(GroupChatMessageCreateApiDto dto, Guid userId)
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
        return new Success<Guid?>(id);
    }

    private Dictionary<string, string[]> ValidateRequest(GroupChatMessageCreateApiDto? request)
    {
        var validationErrors = new Dictionary<string, string[]>();

        if (request is null)
        {
            validationErrors.Add(nameof(GroupChatMessageCreateApiDto), ["Request body not set"]);
            return validationErrors;
        }

        var maxContentLength = 2000;
        if (request.Content.Length > maxContentLength)
        {
            validationErrors.Add(nameof(GroupChatMessageCreateApiDto.Content), ["Message content maximum length is 2000 characters"]);
        }

        return validationErrors;
    }

    private async Task<Dictionary<string, string[]>> Authorize(GroupChatMessageCreateApiDto request, Guid userId)
    {
        var errors = new Dictionary<string, string[]>();

        var chat = await _getGroupChatByIdRepository.Get(request.ChatId);
        if (chat is null)
        {
            errors.Add(nameof(GroupChatMessageCreateApiDto.ChatId), ["Group chat with specified id not found"]);
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
