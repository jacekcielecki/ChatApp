using ChatApp.Domain.Entities;

namespace ChatApp.Domain.Interfaces.Repositories;

public interface IMessageRepository
{
    Task<(IEnumerable<Message>, int)> GetPaged(Guid chatId, uint skip, uint take);
    Task<Message?> GetById(Guid id);
    Task<Guid?> Insert(Message message);
    Task Update(Message message);
    Task Delete(Guid id);
}
