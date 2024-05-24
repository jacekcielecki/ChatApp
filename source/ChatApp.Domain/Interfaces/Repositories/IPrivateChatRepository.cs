using ChatApp.Domain.Entities;

namespace ChatApp.Domain.Interfaces.Repositories;

public interface IPrivateChatRepository
{
    Task<IEnumerable<PrivateChat>> Get(Guid userId);
    Task<PrivateChat?> GetByUserId(Guid receiverId, Guid userId);
    Task<PrivateChat?> Insert(PrivateChat privateChat);
}
