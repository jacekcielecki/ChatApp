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
    private readonly IUserService _userService;

    public MessageService(
        IMessageRepository messageRepository,
        IGroupChatRepository groupChatRepository,
        IPrivateChatRepository privateChatRepository,
        IUserService userService)
    {
        _messageRepository = messageRepository;
        _groupChatRepository = groupChatRepository;
        _privateChatRepository = privateChatRepository;
        _userService = userService;
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

        var receiver = await _userService.GetByEmail(request.ReceiverEmail);
        if (receiver == null)
        {
            validationErrors.Add("ReceiverEmail", ["User with given email not found"]);
            return new ValidationErrors(validationErrors);
        }
        if (receiver.Id == user.Id)
        {
            validationErrors.Add("ReceiverEmail", ["ReceiverId cannot be equal to SenderId"]);
            return new ValidationErrors(validationErrors);
        }

        var chat = await _privateChatRepository.GetByUserId(receiver.Id, user.Id);
        if (chat == null)
        {
            var privateChat = new PrivateChat
            {
                Id = Guid.NewGuid(),
                CreatedAt = DateTime.UtcNow,
                FirstUserId = user.Id,
                SecondUserId = receiver.Id,
                Messages = []
            };

            chat = await _privateChatRepository.Insert(privateChat);
        }
        if (chat == null)
        {
            validationErrors.Add("ReceiverEmail", ["Failed to create chat with given user"]);
            return new ValidationErrors(validationErrors);
        }

        var message = new Message
        {
            Id = Guid.NewGuid(),
            ChatId = chat.Id,
            Content = request.Content,
            CreatedAt = DateTime.UtcNow,
            CreatedById = user.Id
        };

        await _messageRepository.Insert(message);
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
