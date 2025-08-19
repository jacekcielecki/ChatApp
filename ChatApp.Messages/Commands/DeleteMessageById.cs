using ChatApp.Messages.Core.Details;
using ChatApp.Shared.Data.Adapters.Entities;
using ChatApp.Shared.Model.ValueObjects;
using OneOf;
using OneOf.Types;

namespace ChatApp.Messages.Commands;

public class DeleteMessageById
{
    private readonly DeleteMessageByIdRepository _deleteMessageByIdRepository;
    private readonly GetMessageByIdRepository _getMessageByIdRepository;

    public DeleteMessageById(DeleteMessageByIdRepository deleteMessageByIdRepository, GetMessageByIdRepository getMessageByIdRepository)
    {
        _deleteMessageByIdRepository = deleteMessageByIdRepository;
        _getMessageByIdRepository = getMessageByIdRepository;
    }

    public async Task<OneOf<Success, NotFound, Forbidden>> Delete(Guid messageId, Guid userId)
    {
        var message = await _getMessageByIdRepository.Get(messageId);
        if (message is null)
        {
            return new NotFound();
        }

        var authorizationErrors = Authorize(message, userId);
        if (authorizationErrors.Any())
        {
            return new Forbidden(authorizationErrors);
        }

        await _deleteMessageByIdRepository.Delete(messageId);
        return new Success();
    }

    private Dictionary<string, string[]> Authorize(Message message, Guid userId)
    {
        var errors = new Dictionary<string, string[]>();

        if (message.CreatedById != userId)
        {
            errors.Add(nameof(Message.CreatedById), ["User is not allowed to delete specified message"]);
            return errors;
        }

        return errors;
    }
}
