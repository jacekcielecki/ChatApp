using ChatApp.Chats.Core.Group;
using ChatApp.Chats.Core.Members;
using ChatApp.Shared.Data.Adapters.Entities;
using ChatApp.Shared.Model.Chats;
using ChatApp.Shared.Model.ValueObjects;
using OneOf;
using OneOf.Types;

namespace ChatApp.Chats.Commands;

public class CreateGroupChat
{
    private readonly GetUserByIdRepository _getUserByIdRepository;
    private readonly CreateGroupChatRepository _createGroupChatRepository;
    private readonly AddUsersToGroupChatRepository _addUsersToGroupChatRepository;

    public CreateGroupChat(
        GetUserByIdRepository getUserByIdRepository,
        CreateGroupChatRepository createGroupChatRepository,
        AddUsersToGroupChatRepository addUsersToGroupChatRepository)
    {
        _getUserByIdRepository = getUserByIdRepository;
        _createGroupChatRepository = createGroupChatRepository;
        _addUsersToGroupChatRepository = addUsersToGroupChatRepository;
    }

    public async Task<OneOf<Success<Guid?>, ValidationErrors>> Create(GroupChatCreateApiDto dto, Guid userId)
    {
        var validationErrors = await ValidateRequest(dto);
        if (validationErrors.Any())
        {
            return new ValidationErrors(validationErrors);
        }

        var chat = new GroupChat
        {
            Id = Guid.NewGuid(),
            Name = dto.Name,
            CreatedAt = DateTime.UtcNow,
            CreatedById = userId,
            Members = [],
            Messages = []
        };

        var members = dto.Members
            .Append(userId)
            .Distinct()
            .ToList();

        await _createGroupChatRepository.Create(chat);
        await _addUsersToGroupChatRepository.Add(members, chat.Id);

        return new Success<Guid?>(chat.Id);
    }

    private async Task<Dictionary<string, string[]>> ValidateRequest(GroupChatCreateApiDto? request)
    {
        var validationErrors = new Dictionary<string, string[]>();

        if (request is null)
        {
            validationErrors.Add(nameof(GroupChatCreateApiDto), ["Request body not set"]);
            return validationErrors;
        }

        const int minNameLength = 5;
        if (request.Name.Length < minNameLength)
        {
            validationErrors.Add(nameof(GroupChatCreateApiDto.Name), ["Chat name has to be at least 5 character long"]);
        }

        foreach (var memberId in request.Members)
        {
            var member = await _getUserByIdRepository.Get(memberId);
            if (member is null)
            {
                validationErrors.Add(nameof(GroupChatCreateApiDto.Members), [$"Chat member with id {memberId} does not exist"]);
            }
        }

        return validationErrors;
    }
}
