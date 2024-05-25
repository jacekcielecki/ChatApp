using ChatApp.Domain.Entities;

namespace ChatApp.Domain.Interfaces.Repositories;

public interface IMessageRepository
{
    Task<Message?> GetById(Guid id);
    Task Insert(Message message);
    Task Delete(Guid id);
}
