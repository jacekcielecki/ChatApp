using ChatApp.Application.Services;
using ChatApp.Contracts.Request;
using ChatApp.DbUp;
using ChatApp.Infrastructure.Database.DbConnectionFactory;
using ChatApp.Infrastructure.Repositories;


namespace ChatApp.UnitTests.Services;

public sealed class UserServiceTests : IAsyncLifetime
{
    private readonly PostgreSqlContainer _postgres = new PostgreSqlBuilder()
        .WithImage("postgres:15-alpine")
        .Build();

    [Fact]
    public async Task GettingUser_WhenUserWithGivenEmailFound_ReturnsUser()
    {
        // Arrange
        var email = "johny@mail.com";
        var userService = new UserService(new UserRepository(new DbConnectionFactory(_postgres.GetConnectionString())));
        await userService.Create(new CreateUserRequest(email));

        // Act
        var result = await userService.GetByEmail(email);

        // Assert
        result.Should().NotBeNull();
        result?.Email.Should().Be(email);
    }

    public async Task InitializeAsync()
    {
        await _postgres.StartAsync();

        var dbUp = new DatabaseUpdater(_postgres.GetConnectionString());
        dbUp.UpdateDatabase();
    }

    public Task DisposeAsync()
    {
        return _postgres.DisposeAsync().AsTask();
    }
}