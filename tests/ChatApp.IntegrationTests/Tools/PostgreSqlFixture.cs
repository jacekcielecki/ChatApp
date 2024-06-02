using ChatApp.DbUp;
using Npgsql;
using Respawn;
using Testcontainers.PostgreSql;

namespace ChatApp.IntegrationTests.Tools;

public class PostgreSqlFixture : IAsyncLifetime
{
    private PostgreSqlContainer PostgreSqlContainer { get; set; } = null!;
    private Respawner _respawner = default!;
    private NpgsqlConnection _dbConnection = default!;

    public string ConnectionString => PostgreSqlContainer.GetConnectionString();

    public async Task InitializeAsync()
    {
        PostgreSqlContainer = new PostgreSqlBuilder()
            .WithImage("postgres:15-alpine")
            .WithName($"test-db-{Guid.NewGuid()}")
            .Build();

        await PostgreSqlContainer.StartAsync();

        var dbUp = new DatabaseUpdater(PostgreSqlContainer.GetConnectionString());
        dbUp.UpdateDatabase();

        await InitRespawner();
    }

    public async Task ResetDatabase()
    {
        await _respawner.ResetAsync(_dbConnection);
    }

    private async Task InitRespawner()
    {
        _dbConnection = new NpgsqlConnection(ConnectionString);
        _dbConnection.Open();

        _respawner = await Respawner.CreateAsync(_dbConnection, new RespawnerOptions
        {
            DbAdapter = DbAdapter.Postgres,
            SchemasToInclude = ["public"]
        });
    }

    public async Task DisposeAsync()
    {
        await PostgreSqlContainer.StopAsync();
    }
}
