using ChatApp.Application.Interfaces;
using ChatApp.Contracts.Request;
using ChatApp.Domain.Entities;
using ChatApp.Domain.Interfaces.Repositories;
using ChatApp.Domain.ResultTypes;
using OneOf;
using OneOf.Types;

namespace ChatApp.Application.Services;

public class MessageService : IMessageService
{
    private readonly IMessageRepository _messageRepository;
    private readonly IGroupChatRepository _groupChatRepository;
    private readonly IPrivateChatRepository _privateChatRepository;

    public MessageService(
        IMessageRepository messageRepository,
        IGroupChatRepository groupChatRepository,
        IPrivateChatRepository privateChatRepository)
    {
        _messageRepository = messageRepository;
        _groupChatRepository = groupChatRepository;
        _privateChatRepository = privateChatRepository;
    }

    public async Task<OneOf<Success, ValidationErrors>> CreateGroup(CreateGroupMessageRequest request, User user)
    {
        var validationErrors = new Dictionary<string, string[]>();

        var chat = await _groupChatRepository.GetById(request.ChatId);
        if (chat == null)
        {
            validationErrors.Add("ChatId", ["Group chat with given id not found"]);
            return new ValidationErrors(validationErrors);
        }

        var message = new Message
        {
            Id = Guid.NewGuid(),
            ChatId = request.ChatId,
            Content = request.Content,
            CreatedAt = DateTime.UtcNow,
            CreatedById = user.Id
        };

        await _messageRepository.Insert(message);
        return new Success();
    }

    public async Task<OneOf<Success, ValidationErrors>> CreatePrivate(CreatePrivateMessageRequest request, User user)
    {
        var validationErrors = new Dictionary<string, string[]>();

        var chat = await _privateChatRepository.GetById(request.ChatId);
        if (chat == null)
        {
            validationErrors.Add("ChatId", ["Private chat with given id not found"]);
            return new ValidationErrors(validationErrors);
        }

        var message = new Message
        {
            Id = Guid.NewGuid(),
            ChatId = request.ChatId,
            Content = request.Content,
            CreatedAt = DateTime.UtcNow,
            CreatedById = user.Id
        };

        await _messageRepository.Insert(message);
        return new Success();
    }

    public async Task<OneOf<Success, ValidationErrors>> Update(UpdateMessageRequest request, User user)
    {
        var validationErrors = new Dictionary<string, string[]>();

        var message = await _messageRepository.GetById(request.Id);
        if (message == null)
        {
            validationErrors.Add("Id", ["Message with given id not found"]);
            return new ValidationErrors(validationErrors);
        }
        if (message.CreatedById != user.Id)
        {
            validationErrors.Add("Id", ["Unable to update message: user is not message creator"]);
            return new ValidationErrors(validationErrors);
        }

        message.Content = request.Content;

        await _messageRepository.Update(message);
        return new Success();
    }

    public async Task<OneOf<Success, ValidationErrors>> Delete(Guid id, User user)
    {
        var validationErrors = new Dictionary<string, string[]>();

        var message = await _messageRepository.GetById(id);
        if (message == null)
        {
            validationErrors.Add("MessageId", ["Message with given id not found"]);
            return new ValidationErrors(validationErrors);
        }
        if (message.CreatedById != user.Id)
        {
            validationErrors.Add("MessageId", ["User is not the author of the message"]);
            return new ValidationErrors(validationErrors);
        }

        await _messageRepository.Delete(id);
        return new Success();
    }
}
