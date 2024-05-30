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

    public async Task<OneOf<Success, NotFound, ValidationErrors>> CreateGroup(CreateGroupMessageRequest request, User user)
    {
        var validationErrors = new Dictionary<string, string[]>();
        if (request.Content.Length > 2000)
        {
            validationErrors.Add("CreateGroupMessageRequest.Content", ["Message content maximum length is 2000 characters"]);
            return new ValidationErrors(validationErrors);
        }

        var chat = await _groupChatRepository.GetById(request.ChatId);
        if (chat == null)
        {
            return new NotFound();
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

    public async Task<OneOf<Success, NotFound, ValidationErrors>> CreatePrivate(CreatePrivateMessageRequest request, User user)
    {
        var validationErrors = new Dictionary<string, string[]>();
        if (request.Content.Length > 2000)
        {
            validationErrors.Add("CreateGroupMessageRequest.Content", ["Message content maximum length is 2000 characters"]);
            return new ValidationErrors(validationErrors);
        }

        var chat = await _privateChatRepository.GetById(request.ChatId);
        if (chat == null)
        {
            return new NotFound();
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

    public async Task<OneOf<Success, NotFound, Forbidden, ValidationErrors>> Update(UpdateMessageRequest request, User user)
    {
        var validationErrors = new Dictionary<string, string[]>();
        if (request.Content.Length > 2000)
        {
            validationErrors.Add("UpdateMessageRequest.Content", ["Message content maximum length is 2000 characters"]);
            return new ValidationErrors(validationErrors);
        }

        var message = await _messageRepository.GetById(request.Id);
        if (message == null)
        {
            return new NotFound();
        }
        if (message.CreatedById != user.Id)
        {
            return new Forbidden();
        }

        message.Content = request.Content;

        await _messageRepository.Update(message);
        return new Success();
    }

    public async Task<OneOf<Success, NotFound, Forbidden>> Delete(Guid id, User user)
    {
        var message = await _messageRepository.GetById(id);
        if (message == null)
        { 
            return new NotFound();
        }
        if (message.CreatedById != user.Id)
        {
            return new Forbidden();
        }

        await _messageRepository.Delete(id);
        return new Success();
    }
}
