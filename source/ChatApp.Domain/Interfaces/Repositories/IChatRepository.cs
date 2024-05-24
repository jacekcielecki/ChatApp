using ChatApp.Domain.Entities;

namespace ChatApp.Domain.Interfaces.Repositories;

public interface IChatRepository
{
    Task<IEnumerable<GroupChat>> GetGroupChats(Guid userId);
    Task<GroupChat?> GetGroupChatById(Guid id);
    Task<Guid?> CreateGroup(string name, List<Guid> members, Guid creatorId);
}
