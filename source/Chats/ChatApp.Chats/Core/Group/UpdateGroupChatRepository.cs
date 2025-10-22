using ChatApp.Chats.Core.Members;
using ChatApp.Shared.Data.Adapters.DbConnectionFactory;
using ChatApp.Shared.Data.Adapters.Entities;
using Dapper;

namespace ChatApp.Chats.Core.Group;

public class UpdateGroupChatRepository
{
    private readonly IDbConnectionFactory _dbConnectionFactory;
    private readonly AddUsersToGroupChatRepository _addUsersToGroupChatRepository;
    private readonly RemoveUsersFromGroupChatRepository _removeUsersFromGroupChatRepository;

    public UpdateGroupChatRepository(
        IDbConnectionFactory dbConnectionFactory,
        AddUsersToGroupChatRepository addUsersToGroupChatRepository,
        RemoveUsersFromGroupChatRepository removeUsersFromGroupChatRepository)
    {
        _dbConnectionFactory = dbConnectionFactory;
        _addUsersToGroupChatRepository = addUsersToGroupChatRepository;
        _removeUsersFromGroupChatRepository = removeUsersFromGroupChatRepository;
    }

    public async Task Update(GroupChat groupChat)
    {
        DefaultTypeMap.MatchNamesWithUnderscores = true;

        const string sql =
            """
            UPDATE group_chats
            SET name = @name
            WHERE id = @id;
            """;

        await using var connection = _dbConnectionFactory.Create();

        await connection.ExecuteScalarAsync(sql, new { name = groupChat.Name, id = groupChat.Id });

        await _removeUsersFromGroupChatRepository.Delete(groupChat.Id);
        await _addUsersToGroupChatRepository.Add(groupChat.Members.Select(x => x.Id).ToList(), groupChat.Id);
    }
}
