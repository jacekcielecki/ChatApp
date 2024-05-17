using ChatApp.Domain.Entities;

namespace ChatApp.Domain.Interfaces.Repositories;

public interface IChatRepository
{
    Task<IEnumerable<GroupChat>> GetGroupChats(Guid userId);
    Task<Guid?> CreateGroup(string name, List<Guid> members, Guid creatorId);
}
