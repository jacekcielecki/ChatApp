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

    public async Task<GetChatResponse> Get(Guid userId)
    {
        var privateChats = _getPrivateChatsRepository.Get(userId);
        var groupChats = _getGroupChatsRepository.Get(userId);

        await Task.WhenAll(groupChats, privateChats);

        var response = new GetChatResponse(
            privateChats.Result.ToResponse(),
            groupChats.Result.ToResponse());

        return response;
    }
}
