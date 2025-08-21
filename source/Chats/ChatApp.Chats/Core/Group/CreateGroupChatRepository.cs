using ChatApp.Chats.Core.Members;
using ChatApp.Shared.Data.Adapters.DbConnectionFactory;
using ChatApp.Shared.Data.Adapters.Entities;
using Dapper;

namespace ChatApp.Chats.Core.Group;

public class CreateGroupChatRepository
{
    private readonly IDbConnectionFactory _dbConnectionFactory;
    private readonly AddUsersToGroupChatRepository _addUsersToGroupChatRepository;

    public CreateGroupChatRepository(IDbConnectionFactory dbConnectionFactory, AddUsersToGroupChatRepository addUsersToGroupChatRepository)
    {
        _dbConnectionFactory = dbConnectionFactory;
        _addUsersToGroupChatRepository = addUsersToGroupChatRepository;
    }

    public async Task<Guid?> Create(GroupChat chat)
    {
        DefaultTypeMap.MatchNamesWithUnderscores = true;

        const string sql =
            """
            INSERT INTO group_chats (id, name, created_at, created_by_id)
            VALUES (@id, @name, @created_at, @created_by_id)
            RETURNING id;
            """;

        await using var connection = _dbConnectionFactory.Create();
        var chatId = await connection.QuerySingleOrDefaultAsync<Guid>(sql, new
        {
            id = chat.Id,
            name = chat.Name,
            created_at = chat.CreatedAt,
            created_by_id = chat.CreatedById
        });

        await _addUsersToGroupChatRepository.Add(chat.Members, chatId);

        return chatId;
    }
}
