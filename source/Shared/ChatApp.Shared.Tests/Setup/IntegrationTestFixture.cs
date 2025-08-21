using ChatApp.Chats;
using ChatApp.Messages;
using ChatApp.Shared.Data.Adapters.DbConnectionFactory;
using ChatApp.Users;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Testcontainers.PostgreSql;
using Xunit;

namespace ChatApp.Shared.Tests.Setup;

public class IntegrationTestFixture : IAsyncLifetime
{
    private readonly PostgreSqlContainer _postgreSqlContainer;
    private IHost? _host;

    public IntegrationTestFixture()
    {
        _postgreSqlContainer = new PostgreSqlBuilder()
            .WithImage("postgres:15-alpine")
            .WithName($"{nameof(ChatApp)}-Test-Db-{Guid.NewGuid()}")
            .Build();
    }

    public async Task InitializeAsync()
    {
        await _postgreSqlContainer.StartAsync();

        await CreateDatabase();

        var dbConnectionString = _postgreSqlContainer.GetConnectionString();

        _host = new HostBuilder()
            .ConfigureServices(services =>
            {
                services.RegisterChatsCore();
                services.RegisterMessagesCore();
                services.RegisterUsersCore();

                services.Remove(services.First(d => d.ServiceType == typeof(IDbConnectionFactory)));
                services.AddTransient<IDbConnectionFactory, DbConnectionFactory>(_ => new DbConnectionFactory(dbConnectionString));

            }).Build();
    }

    public T ResolveService<T>() => _host!.Services.GetService<T>()!;

    public async Task DisposeAsync()
    {
        await _postgreSqlContainer.StopAsync();
    }

    private async Task CreateDatabase()
    {
        var scripts = Directory.GetFiles($"{AppDomain.CurrentDomain.BaseDirectory}\\Setup\\Database").ToList();

        foreach (var file in scripts)
        {
            var sql = await File.ReadAllTextAsync(file);

            await _postgreSqlContainer.ExecScriptAsync(sql);
        }
    }
}
