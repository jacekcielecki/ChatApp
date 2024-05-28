using ChatApp.Domain.Entities;

namespace ChatApp.Domain.Interfaces.Repositories;

public interface IGroupChatRepository
{
    Task<IEnumerable<GroupChat>> Get(Guid userId);
    Task<GroupChat?> GetById(Guid id);
    Task<Guid?> Insert(GroupChat groupChat);
    Task Update(GroupChat groupChat);
}
