using ChatApp.Domain.Entities;

namespace ChatApp.Domain.Interfaces.Repositories;

public interface IGroupChatMembersRepository
{
    Task BulkInsert(List<User> members, Guid groupChatId);
    Task DeleteByChatId(Guid groupChatId);
}
