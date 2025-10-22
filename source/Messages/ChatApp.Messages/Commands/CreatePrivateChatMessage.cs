using ChatApp.Chats.Core.Private;
using ChatApp.Messages.Core.Details;
using ChatApp.Shared.Data.Adapters.Entities;
using ChatApp.Shared.Model.Messages;
using ChatApp.Shared.Model.ValueObjects;
using OneOf;
using OneOf.Types;

namespace ChatApp.Messages.Commands;

public class CreatePrivateChatMessage
{
    private readonly GetPrivateChatByIdRepository _getPrivateChatByIdRepository;
    private readonly CreateMessageRepository _createMessageRepository;

    public CreatePrivateChatMessage(GetPrivateChatByIdRepository getPrivateChatByIdRepository, CreateMessageRepository createMessageRepository)
    {
        _getPrivateChatByIdRepository = getPrivateChatByIdRepository;
        _createMessageRepository = createMessageRepository;
    }

    public async Task<OneOf<Success<Guid?>, Forbidden, ValidationErrors>> Create(PrivateChatCreateApiDto dto, Guid userId)
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

    private Dictionary<string, string[]> ValidateRequest(PrivateChatCreateApiDto? request)
    {
        var validationErrors = new Dictionary<string, string[]>();

        if (request is null)
        {
            validationErrors.Add(nameof(PrivateChatCreateApiDto), ["Request body not set"]);
            return validationErrors;
        }

        var maxContentLength = 2000;
        if (request.Content.Length > maxContentLength)
        {
            validationErrors.Add(nameof(PrivateChatCreateApiDto.Content), ["Message content maximum length is 2000 characters"]);
        }

        return validationErrors;
    }

    private async Task<Dictionary<string, string[]>> Authorize(PrivateChatCreateApiDto request, Guid userId)
    {
        var errors = new Dictionary<string, string[]>();

        var chat = await _getPrivateChatByIdRepository.Get(request.ChatId);
        if (chat is null)
        {
            errors.Add(nameof(PrivateChatCreateApiDto.ChatId), ["Private chat with specified id not found"]);
            return errors;
        }

        if (userId != chat.FirstUserId && userId != chat.SecondUserId)
        {
            errors.Add(nameof(PrivateChatCreateApiDto.ChatId), ["This user is not a member of specified chat"]);
            return errors;
        }

        return errors;
    }
}
