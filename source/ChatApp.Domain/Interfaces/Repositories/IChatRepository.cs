using ChatApp.Contracts.Request;

namespace ChatApp.Domain.Interfaces.Repositories;

public interface IChatRepository
{
    Task<Guid?> CreateGroup(CreateGroupChatRequest request, Guid creatorId);
}
