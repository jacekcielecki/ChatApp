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

    public CreateGroupChat(GetUserByIdRepository getUserByIdRepository, CreateGroupChatRepository createGroupChatRepository)
    {
        _getUserByIdRepository = getUserByIdRepository;
        _createGroupChatRepository = createGroupChatRepository;
    }

    public async Task<OneOf<Success<Guid?>, ValidationErrors>> Create(CreateGroupChatRequest request, Guid userId)
    {
        var validationErrors = await ValidateRequest(request);
        if (validationErrors.Any())
        {
            return new ValidationErrors(validationErrors);
        }

        var chat = new GroupChat
        {
            Id = Guid.NewGuid(),
            Name = request.Name,
            CreatedAt = DateTime.UtcNow,
            CreatedById = userId,
            Members = [],
            Messages = []
        };

        var members = request.Members
            .Append(userId)
            .Distinct()
            .Select(x => new User{ Id = x })
            .ToList();

        chat.Members = members;

        var id = await _createGroupChatRepository.Create(chat);
        return new Success<Guid?>(id);
    }

    private async Task<Dictionary<string, string[]>> ValidateRequest(CreateGroupChatRequest? request)
    {
        var validationErrors = new Dictionary<string, string[]>();

        if (request is null)
        {
            validationErrors.Add(nameof(CreateGroupChatRequest), ["Request body not set"]);
            return validationErrors;
        }

        const int minNameLength = 5;
        if (request.Name.Length < minNameLength)
        {
            validationErrors.Add(nameof(CreateGroupChatRequest.Name), ["Chat name has to be at least 5 character long"]);
        }

        foreach (var memberId in request.Members)
        {
            var member = await _getUserByIdRepository.Get(memberId);
            if (member is null)
            {
                validationErrors.Add(nameof(CreateGroupChatRequest.Members), [$"Chat member with id {memberId} does not exist"]);
            }
        }

        return validationErrors;
    }
}
