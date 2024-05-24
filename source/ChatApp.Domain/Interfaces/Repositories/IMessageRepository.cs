using ChatApp.Domain.Entities;

namespace ChatApp.Domain.Interfaces.Repositories;

public interface IMessageRepository
{
    Task Insert(Message message);
}
