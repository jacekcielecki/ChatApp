using ChatApp.Application.Interfaces;
using ChatApp.Contracts.Request;
using ChatApp.Domain.Interfaces.Repositories;
using ChatApp.Domain.ResultTypes;
using OneOf;
using OneOf.Types;

namespace ChatApp.Application.Services;

public class ChatService : IChatService
{
    private readonly IChatRepository _chatRepository;
    private readonly IUserService _userService;

    public ChatService(IChatRepository chatRepository, IUserService userService)
    {
        _chatRepository = chatRepository;
        _userService = userService;
    }

    public async Task<OneOf<Success<Guid>, ValidationErrors>> CreateGroup(CreateGroupChatRequest request, Guid senderId)
    {
        Dictionary<string, string[]> validationErrors = new();

        if (request.Name.Length < 5)
        {
            validationErrors.Add("Name", ["Chat name has to be at least 5 character long"]);
        }
        foreach (var memberId in request.Members)
        {
            var user = await _userService.GetById(memberId);
            if (user == null)
            {
                validationErrors.Add("ReceiverId", [$"Chat member with id {memberId} not found"]);
                break;
            }
        }

        if (validationErrors.Any())
        {
            return new ValidationErrors(validationErrors);
        }

        var chatId = await _chatRepository.CreateGroup(request, senderId);
        if (chatId != null)
        {
            return new Success<Guid>(chatId.Value);
        }

        return new ValidationErrors(validationErrors);
    }
}