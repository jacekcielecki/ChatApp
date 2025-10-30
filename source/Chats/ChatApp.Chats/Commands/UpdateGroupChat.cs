using ChatApp.Chats.Core.Group;
using ChatApp.Chats.Core.Members;
using ChatApp.Shared.Data.Adapters.Entities;
using ChatApp.Shared.Model.Chats;
using ChatApp.Shared.Model.ValueObjects;
using OneOf;
using OneOf.Types;

namespace ChatApp.Chats.Commands;

public class UpdateGroupChat
{
    private readonly GetGroupChatByIdRepository _getGroupChatByIdRepository;
    private readonly GetUserByIdRepository _getUserByIdRepository;
    private readonly UpdateGroupChatRepository _updateGroupChatRepository;

    public UpdateGroupChat(
        GetGroupChatByIdRepository getGroupChatByIdRepository,
        GetUserByIdRepository getUserByIdRepository,
        UpdateGroupChatRepository updateGroupChatRepository)
    {
        _getGroupChatByIdRepository = getGroupChatByIdRepository;
        _getUserByIdRepository = getUserByIdRepository;
        _updateGroupChatRepository = updateGroupChatRepository;
    }

    public async Task<OneOf<Success, NotFound, Forbidden, ValidationErrors>> Update(GroupChatUpdateApiDto dto, Guid userId)
    {
        var chat = await _getGroupChatByIdRepository.Get(dto.Id);
        if (chat is null)
        {
            return new NotFound();
        }

        var validationErrors = await ValidateRequest(dto);
        if (validationErrors.Any())
        {
            return new ValidationErrors(validationErrors);
        }

        var authorizationErrors = Authorize(chat, userId);
        if (authorizationErrors.Any())
        {
            return new Forbidden(authorizationErrors);
        }

        chat.Members.Clear();

        var members = dto.Members
            .Distinct()
            .ToList();

        chat.Name = dto.Name;

        foreach (var memberId in members)
        {
            var member = await _getUserByIdRepository.Get(memberId);
            if (member is not null)
            {
                chat.Members.Add(member);
            }
        }

        await _updateGroupChatRepository.Update(chat);
        return new Success();
    }

    private async Task<Dictionary<string, string[]>> ValidateRequest(GroupChatUpdateApiDto request)
    {
        var validationErrors = new Dictionary<string, string[]>();

        foreach (var memberId in request.Members)
        {
            var member = await _getUserByIdRepository.Get(memberId);
            if (member is null)
            {
                validationErrors.Add(nameof(GroupChatUpdateApiDto.Members), [$"User with id {memberId} not found"]);
            }
        }

        return validationErrors;
    }

    private Dictionary<string, string[]> Authorize(GroupChat chat, Guid userId)
    {
        var authorizationErrors = new Dictionary<string, string[]>();

        if (chat.Members.All(x => x.Id != userId) && chat.CreatedById != userId)
        {
            authorizationErrors.Add(nameof(GroupChat.Members), ["User cannot update specified chat"]);
        }

        return authorizationErrors;
    }
}
