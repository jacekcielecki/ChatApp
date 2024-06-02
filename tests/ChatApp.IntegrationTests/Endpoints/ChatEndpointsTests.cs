using ChatApp.Contracts.Request;
using ChatApp.Contracts.Response;
using ChatApp.Domain.Entities;
using ChatApp.Domain.Interfaces.Repositories;
using ChatApp.IntegrationTests.Tools;
using Microsoft.Extensions.DependencyInjection;
using System.Net;
using System.Net.Http.Json;

namespace ChatApp.IntegrationTests.Endpoints;

[Collection("PostgreSql collection")]
public sealed class ChatEndpointsTests : IAsyncLifetime
{
    private readonly PostgreSqlFixture _postgreFixture;

    public ChatEndpointsTests(PostgreSqlFixture postgreFixture)
    {
        _postgreFixture = postgreFixture;
    }

    [Fact]
    public async Task GettingCurrentUserChats_WhenAuthenticated_ReturnsUserChats()
    {
        // Arrange
        var fixture = new TestApiFixture(_postgreFixture);
        using var client = fixture.CreateClient();

        // Act
        using var result = await client.GetAsync("api/chats/me");

        // Assert
        result.StatusCode.Should().Be(HttpStatusCode.OK);
        var content = await result.Content.ReadFromJsonAsync<GetChatResponse>();
        content.Should().NotBeNull();
    }

    [Fact]
    public async Task CreatingGroupChat_WithValidRequest_Returns200Ok()
    {
        // Arrange
        var fixture = new TestApiFixture(_postgreFixture);
        using var client = fixture.CreateClient();
        var request = new CreateGroupChatRequest(Name: "test group chat 1", []);

        // Act
        using var result = await client.PostAsJsonAsync("api/chats/group", request);

        // Assert
        result.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task CreatingPrivateChat_WithValidRequest_Returns200Ok()
    {
        // Arrange
        var fixture = new TestApiFixture(_postgreFixture);
        using var client = fixture.CreateClient();

        var userRepository = fixture.Services.GetRequiredService<IUserRepository>();
        var existingUser = new User
        {
            Id = Guid.NewGuid(),
            CreatedAt = DateTime.Now,
            Email = "johny@testUser@mai.com"
        };
        await userRepository.Insert(existingUser);

        var request = new CreatePrivateChatRequest(ReceiverId: existingUser.Id);

        // Act
        using var result = await client.PostAsJsonAsync("api/chats/private", request);

        // Assert
        result.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task UpdatingGroupChat_WithValidRequest_Returns200Ok()
    {
        // Arrange
        var fixture = new TestApiFixture(_postgreFixture);
        using var client = fixture.CreateClient();

        var userRepository = fixture.Services.GetRequiredService<IUserRepository>();
        var existingUser = new User
        {
            Id = Guid.NewGuid(),
            CreatedAt = DateTime.Now,
            Email = TestApiFixture.TestUserEmail
        };
        await userRepository.Insert(existingUser);

        var groupChatRepository = fixture.Services.GetRequiredService<IGroupChatRepository>();
        var existingGroupChat = new GroupChat
        {
            Id = Guid.NewGuid(),
            Name = "test group chat 1",
            CreatedAt = DateTime.Now,
            Members = [existingUser],
            CreatedById = existingUser.Id,
            Messages = []
        };
        await groupChatRepository.Insert(existingGroupChat);

        var request = new UpdateGroupChatRequest(Id: existingGroupChat.Id, "test group chat 1 updated", Members: [existingUser.Id]);

        // Act
        using var result = await client.PutAsJsonAsync("api/chats", request);

        // Assert
        result.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    public Task InitializeAsync() => Task.CompletedTask;

    public async Task DisposeAsync() =>
        await _postgreFixture.ResetDatabase();
}
