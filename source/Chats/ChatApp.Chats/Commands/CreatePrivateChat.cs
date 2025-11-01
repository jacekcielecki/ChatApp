using ChatApp.Chats.Core.Hubs;
using ChatApp.Chats.Core.Members;
using ChatApp.Chats.Core.Private;
using ChatApp.Shared.Data.Adapters.Entities;
using ChatApp.Shared.Model.Chats;
using ChatApp.Shared.Model.ValueObjects;
using Microsoft.AspNetCore.SignalR;
using OneOf;
using OneOf.Types;

namespace ChatApp.Chats.Commands;

public class CreatePrivateChat
{
    private readonly GetUserByIdRepository _getUserByIdRepository;
    private readonly CreatePrivateChatRepository _createPrivateChatRepository;
    private readonly GetPrivateChatByIdRepository _getPrivateChatByIdRepository;
    private readonly IHubContext<ChatHub, IMessageClient> _messageClient;

    public CreatePrivateChat(GetUserByIdRepository getUserByIdRepository,
        CreatePrivateChatRepository createPrivateChatRepository,
        GetPrivateChatByIdRepository getPrivateChatByIdRepository,
        IHubContext<ChatHub, IMessageClient> messageClient
        )
    {
        _getUserByIdRepository = getUserByIdRepository;
        _createPrivateChatRepository = createPrivateChatRepository;
        _getPrivateChatByIdRepository = getPrivateChatByIdRepository;
        _messageClient = messageClient;
    }

    public async Task<OneOf<Success<Guid?>, ValidationErrors>> Create(PrivateChatCreateApiDto dto, Guid userId)
    {
        var validationErrors = await ValidateRequest(dto, userId);
        if (validationErrors.Any())
        {
            return new ValidationErrors(validationErrors);
        }

        var chat = new PrivateChat
        {
            Id = Guid.NewGuid(),
            CreatedAt = DateTime.UtcNow,
            FirstUserId = userId,
            SecondUserId = dto.ReceiverId,
            Messages = []
        };

        var id = await _createPrivateChatRepository.Create(chat);

        var memberIdentifiers = new List<string>
        {
            chat.FirstUserId.ToString(),
            chat.SecondUserId.ToString()
        };

        await _messageClient.Clients.Users(memberIdentifiers).ReceiveChat(chat.ToDto());

        return new Success<Guid?>(id);
    }

    private async Task<Dictionary<string, string[]>> ValidateRequest(PrivateChatCreateApiDto? request, Guid userId)
    {
        var validationErrors = new Dictionary<string, string[]>();

        if (request is null)
        {
            validationErrors.Add(nameof(PrivateChatCreateApiDto), ["Request body not set"]);
            return validationErrors;
        }
        var receiver = await _getUserByIdRepository.Get(request.ReceiverId);
        if (receiver is null)
        {
            validationErrors.Add(nameof(PrivateChatCreateApiDto.ReceiverId), ["Message receiver with given id not found"]);
            return validationErrors;
        }
        if (receiver.Id == userId)
        {
            validationErrors.Add(nameof(PrivateChatCreateApiDto.ReceiverId), ["ReceiverId cannot be equal to CreatorId"]);
        }
        var existingChat = await _getPrivateChatByIdRepository.GetByUserId(request.ReceiverId, userId);
        if (existingChat is not null)
        {
            validationErrors.Add(nameof(PrivateChat.Receiver), ["Chat between those two users already exists"]);
        }

        return validationErrors;
    }
}
