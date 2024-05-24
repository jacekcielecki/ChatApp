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

    public MessageService(IMessageRepository messageRepository, IGroupChatRepository groupChatRepository)
    {
        _messageRepository = messageRepository;
        _groupChatRepository = groupChatRepository;
    }

    public async Task<OneOf<Success, ValidationErrors>> Create(CreateMessageRequest request, User user)
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
}
