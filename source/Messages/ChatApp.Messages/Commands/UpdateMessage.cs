using ChatApp.Messages.Core.Details;
using ChatApp.Shared.Model.Messages;
using ChatApp.Shared.Model.ValueObjects;
using OneOf;
using OneOf.Types;

namespace ChatApp.Messages.Commands;

public class UpdateMessage
{
    private readonly GetMessageByIdRepository _getMessageByIdRepository;
    private readonly UpdateMessageRepository _updateMessageRepository;

    public UpdateMessage(GetMessageByIdRepository getMessageByIdRepository, UpdateMessageRepository updateMessageRepository)
    {
        _getMessageByIdRepository = getMessageByIdRepository;
        _updateMessageRepository = updateMessageRepository;
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
