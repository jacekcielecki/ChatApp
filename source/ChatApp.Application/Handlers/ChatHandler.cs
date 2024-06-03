using ChatApp.Application.Interfaces;
using ChatApp.Contracts.Request;
using ChatApp.Domain.Entities;
using ChatApp.Domain.Interfaces.Repositories;
using ChatApp.Domain.ResultTypes;
using OneOf;
using OneOf.Types;

namespace ChatApp.Application.Handlers;

public class ChatHandler : IChatHandler
{
    private readonly IGroupChatRepository _groupChatRepository;
    private readonly IPrivateChatRepository _privateChatRepository;
    private readonly IUserRepository _userRepository;

    public ChatHandler(
        IGroupChatRepository groupChatRepository,
        IPrivateChatRepository privateChatRepository,
        IUserRepository userRepository)
    {
        _groupChatRepository = groupChatRepository;
        _privateChatRepository = privateChatRepository;
        _userRepository = userRepository;
    }

    public async Task<IEnumerable<GroupChat>> GetGroupChats(User user)
    {
        return await _groupChatRepository.Get(user.Id);
    }

    public async Task<IEnumerable<PrivateChat>> GetPrivateChats(User user)
    {
        return await _privateChatRepository.Get(user.Id);
    }

    public async Task<OneOf<Success<Guid>, ValidationErrors>> CreateGroup(CreateGroupChatRequest request, User user)
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
            CreatedById = user.Id,
            Members = [],
            Messages = []
        };

        var members = request.Members.Append(user.Id).Distinct().ToList();
        foreach (var memberId in members)
        {
            var member = await _userRepository.GetById(memberId);
            if (member == null)
            {
                validationErrors.Add("ReceiverId", [$"Chat member with id {memberId} not found"]);
                break;
            }
            groupChat.Members.Add(member);
        }

        if (validationErrors.Any())
        {
            return new ValidationErrors(validationErrors);
        }

        await _groupChatRepository.Insert(groupChat);
        return new Success<Guid>(groupChat.Id);
    }

    public async Task<OneOf<Success<Guid>, ValidationErrors>> CreatePrivate(CreatePrivateChatRequest request, User user)
    {
        var validationErrors = new Dictionary<string, string[]>();

        var receiver = await _userRepository.GetById(request.ReceiverId);
        if (receiver == null)
        {
            validationErrors.Add("ReceiverId", ["Message receiver with given id not found"]);
            return new ValidationErrors(validationErrors);
        }
        if (receiver.Id == user.Id)
        {
            validationErrors.Add("ReceiverId", ["ReceiverId cannot be equal to CreatorId"]);
            return new ValidationErrors(validationErrors);
        }

        var existingChat = await _privateChatRepository.GetByUserId(request.ReceiverId, user.Id);
        if (existingChat != null)
        {
            validationErrors.Add("Chat", ["Chat between those two users already exists"]);
            return new ValidationErrors(validationErrors);
        }

        var privateChat = new PrivateChat
        {
            Id = Guid.NewGuid(),
            CreatedAt = DateTime.UtcNow,
            FirstUserId = user.Id,
            SecondUserId = receiver.Id,
            Messages = []
        };

        await _privateChatRepository.Insert(privateChat);
        return new Success<Guid>(privateChat.Id);
    }

    public async Task<OneOf<Success, NotFound, Forbidden, ValidationErrors>> UpdateGroup(UpdateGroupChatRequest request, User user)
    {
        var validationErrors = new Dictionary<string, string[]>();

        var groupChat = await _groupChatRepository.GetById(request.Id);
        if (groupChat == null)
        {
            return new NotFound();
        }
        if (groupChat.Members.All(x => x.Id != user.Id))
        {
            return new Forbidden();
        }

        groupChat.Name = request.Name;
        groupChat.Members = [];

        var members = request.Members.Distinct().ToList();
        foreach (var memberId in members)
        {
            var member = await _userRepository.GetById(memberId);
            if (member == null)
            {
                validationErrors.Add("Members", [$"Chat member with id {memberId} not found"]);
                break;
            }
            groupChat.Members.Add(member);
        }

        if (validationErrors.Any())
        {
            return new ValidationErrors(validationErrors);
        }

        await _groupChatRepository.Update(groupChat);
        return new Success();
    }
}