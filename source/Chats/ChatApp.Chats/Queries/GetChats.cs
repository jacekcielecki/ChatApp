using ChatApp.Chats.Core.Group;
using ChatApp.Chats.Core.Private;
using ChatApp.Shared.Model.Chats;

namespace ChatApp.Chats.Queries;

public class GetChats
{
    private readonly GetGroupChatsRepository _getGroupChatsRepository;
    private readonly GetPrivateChatsRepository _getPrivateChatsRepository;

    public GetChats(GetGroupChatsRepository getGroupChatsRepository, GetPrivateChatsRepository getPrivateChatsRepository)
    {
        _getGroupChatsRepository = getGroupChatsRepository;
        _getPrivateChatsRepository = getPrivateChatsRepository;
    }

    public async Task<ContactsAndChatsDto> Get(Guid userId)
    {
        var privateChats = _getPrivateChatsRepository.Get(userId);
        var groupChats = _getGroupChatsRepository.Get(userId);

        await Task.WhenAll(groupChats, privateChats);

        var chats = new List<ChatDto>()
            .Concat(privateChats.Result.Select(x => x.ToDto()))
            .Concat(groupChats.Result.Select(x => x.ToDto()))
            .OrderByDescending(x => x.Messages.FirstOrDefault()?.CreatedAt is null ? x.CreatedAt : x.Messages.FirstOrDefault()?.CreatedAt)
            .ToList();

        var result = new ContactsAndChatsDto
        {
            Chats = chats
                .Where(x => x.Messages.Any())
                .ToList(),
            Contacts = privateChats.Result
                .OrderBy(x => x.Receiver?.GivenName)
                .ThenBy(x => x.Receiver?.FamilyName)
                .ToDictionary(x => x.Receiver!.Id, x => $"{x.Receiver!.GivenName} {x.Receiver.FamilyName}")
        };

        return result;
    }
}
