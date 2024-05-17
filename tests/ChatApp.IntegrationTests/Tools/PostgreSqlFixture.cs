using ChatApp.DbUp;
using Testcontainers.PostgreSql;

namespace ChatApp.IntegrationTests.Tools;

public class PostgreSqlFixture : IAsyncLifetime
{
    private PostgreSqlContainer PostgreSqlContainer { get; set; } = null!;
    public string ConnectionString => PostgreSqlContainer.GetConnectionString();

    public async Task InitializeAsync()
    {
        PostgreSqlContainer = new PostgreSqlBuilder()
            .WithImage("postgres:15-alpine")
            .WithName("test-db")
            .Build();

        await PostgreSqlContainer.StartAsync();

        var dbUp = new DatabaseUpdater(PostgreSqlContainer.GetConnectionString());
        dbUp.UpdateDatabase();
    }

    public async Task DisposeAsync()
    {
        await PostgreSqlContainer.StopAsync();
    }
}
