using ChatApp.Application.Interfaces;
using ChatApp.Contracts.Request;
using ChatApp.Domain.Entities;
using ChatApp.Domain.Interfaces.Repositories;
using ChatApp.Domain.ResultTypes;
using OneOf;
using OneOf.Types;

namespace ChatApp.Application.Services;

public class MessageHandler : IMessageHandler
{
    private readonly IMessageRepository _messageRepository;
    private readonly IGroupChatRepository _groupChatRepository;
    private readonly IPrivateChatRepository _privateChatRepository;

    public MessageHandler(
        IMessageRepository messageRepository,
        IGroupChatRepository groupChatRepository,
        IPrivateChatRepository privateChatRepository)
    {
        _messageRepository = messageRepository;
        _groupChatRepository = groupChatRepository;
        _privateChatRepository = privateChatRepository;
    }

    public async Task<OneOf<Success<(IEnumerable<Message>, int)>, NotFound, Forbidden, ValidationErrors>> GetPaged(GetPagedMessagesRequest request, User user)
    {
        var validationErrors = new Dictionary<string, string[]>();
        if (request.PageSize > 200)
        {
            validationErrors.Add("GetPagedMessagesRequest.PageSize", ["Max page size is equal 200 messages"]);
            return new ValidationErrors(validationErrors);
        }

        var groupChat = await _groupChatRepository.GetById(request.ChatId);
        if (groupChat != null)
        {
            if (groupChat.Members.All(x => x.Id != user.Id))
            {
                return new Forbidden();
            }
        }
        else
        {
            var privateChat = await _privateChatRepository.GetById(request.ChatId);
            if (privateChat != null)
            {
                if (privateChat.FirstUserId != user.Id && privateChat.SecondUserId != user.Id)
                {
                    return new Forbidden();
                }
            }
            else
            {
                return new NotFound();
            }
        }

        var messages = await _messageRepository.GetPaged(request.ChatId, request.PageSize * (request.PageNumber - 1), request.PageSize);
        return new Success<(IEnumerable<Message>, int)>(messages);
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
