using ChatApp.Application.Interfaces;
using ChatApp.Contracts.Request;
using ChatApp.Domain.Entities;
using ChatApp.Domain.Interfaces.Repositories;
using ChatApp.Domain.ResultTypes;
using OneOf;
using OneOf.Types;

namespace ChatApp.Application.Services;

public class ChatService : IChatService
{
    private readonly IGroupChatRepository _groupChatRepository;
    private readonly IPrivateChatRepository _privateChatRepository;
    private readonly IUserService _userService;

    public ChatService(
        IGroupChatRepository groupChatRepository,
        IPrivateChatRepository privateChatRepository,
        IUserService userService)
    {
        _groupChatRepository = groupChatRepository;
        _privateChatRepository = privateChatRepository;
        _userService = userService;
    }

    public async Task<IEnumerable<GroupChat>> GetGroupChats(Guid userId)
    {
        return await _groupChatRepository.Get(userId);
    }

    public async Task<IEnumerable<PrivateChat>> GetPrivateChats(Guid userId)
    {
        return await _privateChatRepository.Get(userId);
    }

    public async Task<OneOf<Success<Guid>, ValidationErrors>> CreateGroup(CreateGroupChatRequest request, Guid creatorId)
    {
        var validationErrors = new Dictionary<string, string[]>();
        if (request.Name.Length < 5)
        {
            validationErrors.Add("Name", ["Chat name has to be at least 5 character long"]);
        }

        var groupChat = new GroupChat
        {
            Id = Guid.NewGuid(),
            Name = request.Name,
            CreatedAt = DateTime.UtcNow,
            CreatedById = creatorId,
            Members = [],
            Messages = []
        };

        var members = request.Members.Append(creatorId).Distinct().ToList();
        foreach (var memberId in members)
        {
            var user = await _userService.GetById(memberId);
            if (user == null)
            {
                validationErrors.Add("ReceiverId", [$"Chat member with id {memberId} not found"]);
                break;
            }
            groupChat.Members.Add(user);
        }

        if (validationErrors.Any())
        {
            return new ValidationErrors(validationErrors);
        }

        await _groupChatRepository.Insert(groupChat);
        return new Success<Guid>(groupChat.Id);
    }

    public async Task<OneOf<Success<Guid>, ValidationErrors>> CreatePrivate(CreatePrivateChatRequest request, Guid creatorId)
    {
        var validationErrors = new Dictionary<string, string[]>();

        var receiver = await _userService.GetById(request.ReceiverId);
        if (receiver == null)
        {
            validationErrors.Add("ReceiverId", ["Message receiver with given id not found"]);
            return new ValidationErrors(validationErrors);
        }
        if (receiver.Id == creatorId)
        {
            validationErrors.Add("ReceiverId", ["ReceiverId cannot be equal to CreatorId"]);
            return new ValidationErrors(validationErrors);
        }

        var existingChat = await _privateChatRepository.GetByUserId(request.ReceiverId, creatorId);
        if (existingChat != null)
        {
            validationErrors.Add("Chat", ["Chat between those two users already exists"]);
            return new ValidationErrors(validationErrors);
        }

        var privateChat = new PrivateChat
        {
            Id = Guid.NewGuid(),
            CreatedAt = DateTime.UtcNow,
            FirstUserId = creatorId,
            SecondUserId = receiver.Id,
            Messages = []
        };

        await _privateChatRepository.Insert(privateChat);
        return new Success<Guid>(privateChat.Id);
    }
}