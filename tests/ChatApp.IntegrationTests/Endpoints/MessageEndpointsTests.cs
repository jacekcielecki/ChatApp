using ChatApp.Contracts.Request;
using ChatApp.Domain.Common;
using ChatApp.Domain.Entities;
using ChatApp.Domain.Interfaces.Repositories;
using ChatApp.IntegrationTests.Tools;
using Microsoft.Extensions.DependencyInjection;
using System.Net;
using System.Net.Http.Json;

namespace ChatApp.IntegrationTests.Endpoints;

[Collection("PostgreSql collection")]
public sealed class MessageEndpointsTests : IAsyncLifetime
{
    private readonly PostgreSqlFixture _postgreFixture;

    public MessageEndpointsTests(PostgreSqlFixture postgreFixture)
    {
        _postgreFixture = postgreFixture;
    }

    [Fact]
    public async Task GettingPaged_WithExistingChat_ReturnsChatMessages()
    {
        // Arrange
        var pageSize = 5;
        var pageNumber = 1;

        var fixture = new TestApiFixture(_postgreFixture);
        using var client = fixture.CreateClient();

        var userRepository = fixture.Services.GetRequiredService<IUserRepository>();
        var authenticatedUser = new User
        {
            Id = Guid.NewGuid(),
            CreatedAt = DateTime.Now,
            Email = TestApiFixture.TestUserEmail
        };
        await userRepository.Insert(authenticatedUser);

        var chatRepository = fixture.Services.GetRequiredService<IGroupChatRepository>();
        var existingChat = new GroupChat
        {
            Id = Guid.NewGuid(),
            Name = "test group chat 1",
            CreatedAt = DateTime.Now,
            Members = [authenticatedUser],
            CreatedById = authenticatedUser.Id,
            Messages = []
        };
        await chatRepository.Insert(existingChat);

        // Act
        using var result = await client.GetAsync(
            $"api/messages/paged/ChatId={existingChat.Id}&PageSize={pageSize}&PageNumber={pageNumber}");

        // Assert
        result.StatusCode.Should().Be(HttpStatusCode.OK);
        var content = await result.Content.ReadFromJsonAsync<PagedResult<Message>>();
        content.Should().NotBeNull();
    }

    [Fact]
    public async Task CreateGroupChatMessage_WithValidRequest_Returns200Ok()
    {
        // Arrange
        var fixture = new TestApiFixture(_postgreFixture);
        using var client = fixture.CreateClient();

        var userRepository = fixture.Services.GetRequiredService<IUserRepository>();
        var authenticatedUser = new User
        {
            Id = Guid.NewGuid(),
            CreatedAt = DateTime.Now,
            Email = TestApiFixture.TestUserEmail
        };
        await userRepository.Insert(authenticatedUser);

        var groupChatRepository = fixture.Services.GetRequiredService<IGroupChatRepository>();
        var existingChat = new GroupChat
        {
            Id = Guid.NewGuid(),
            Name = "test group chat 1",
            CreatedAt = DateTime.Now,
            Members = [],
            CreatedById = authenticatedUser.Id,
            Messages = []
        };
        await groupChatRepository.Insert(existingChat);

        var request = new CreateGroupMessageRequest(ChatId: existingChat.Id, Content: "test message");

        // Act
        using var result = await client.PostAsJsonAsync("api/messages/group", request);

        // Assert
        result.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task CreatePrivateChatMessage_WithValidRequest_Returns200Ok()
    {
        // Arrange
        var fixture = new TestApiFixture(_postgreFixture);
        using var client = fixture.CreateClient();

        var userRepository = fixture.Services.GetRequiredService<IUserRepository>();
        var authenticatedUser = new User
        {
            Id = Guid.NewGuid(),
            CreatedAt = DateTime.Now,
            Email = TestApiFixture.TestUserEmail
        };
        var messageReceiver = new User
        {
            Id = Guid.NewGuid(),
            CreatedAt = DateTime.Now,
            Email = "johny@test.com"
        };
        await userRepository.Insert(authenticatedUser);
        await userRepository.Insert(messageReceiver);

        var privateChatRepository = fixture.Services.GetRequiredService<IPrivateChatRepository>();
        var existingChat = new PrivateChat
        {
            Id = Guid.NewGuid(),
            CreatedAt = DateTime.Now,
            FirstUserId = authenticatedUser.Id,
            SecondUserId = messageReceiver.Id,
            Messages = []
        };
        await privateChatRepository.Insert(existingChat);

        var request = new CreatePrivateMessageRequest(ChatId: existingChat.Id, Content: "test message");

        // Act
        using var result = await client.PostAsJsonAsync("api/messages/private", request);

        // Assert
        result.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task UpdateMessage_WithValidRequest_Returns200Ok()
    {
        // Arrange
        var fixture = new TestApiFixture(_postgreFixture);
        using var client = fixture.CreateClient();

        var userRepository = fixture.Services.GetRequiredService<IUserRepository>();
        var authenticatedUser = new User
        {
            Id = Guid.NewGuid(),
            CreatedAt = DateTime.Now,
            Email = TestApiFixture.TestUserEmail
        };
        var messageReceiver = new User
        {
            Id = Guid.NewGuid(),
            CreatedAt = DateTime.Now,
            Email = "johny@test.com"
        };
        await userRepository.Insert(authenticatedUser);
        await userRepository.Insert(messageReceiver);

        var privateChatRepository = fixture.Services.GetRequiredService<IPrivateChatRepository>();
        var existingChat = new PrivateChat
        {
            Id = Guid.NewGuid(),
            CreatedAt = DateTime.Now,
            FirstUserId = authenticatedUser.Id,
            SecondUserId = messageReceiver.Id,
            Messages = []
        };
        await privateChatRepository.Insert(existingChat);

        var messageRepository = fixture.Services.GetRequiredService<IMessageRepository>();
        var existingMessage = new Message
        {
            Id = Guid.NewGuid(),
            Content = "test message to update",
            CreatedAt = DateTime.Now,
            ChatId = existingChat.Id,
            CreatedById = authenticatedUser.Id
        };
        await messageRepository.Insert(existingMessage);

        var request = new UpdateMessageRequest(Id: existingMessage.Id, Content: "test message after update");

        // Act
        using var result = await client.PutAsJsonAsync("api/messages", request);

        // Assert
        result.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task DeleteMessage_WithValidRequest_Returns200Ok()
    {
        // Arrange
        var fixture = new TestApiFixture(_postgreFixture);
        using var client = fixture.CreateClient();

        var userRepository = fixture.Services.GetRequiredService<IUserRepository>();
        var authenticatedUser = new User
        {
            Id = Guid.NewGuid(),
            CreatedAt = DateTime.Now,
            Email = TestApiFixture.TestUserEmail
        };
        var messageReceiver = new User
        {
            Id = Guid.NewGuid(),
            CreatedAt = DateTime.Now,
            Email = "johny@test.com"
        };
        await userRepository.Insert(authenticatedUser);
        await userRepository.Insert(messageReceiver);

        var privateChatRepository = fixture.Services.GetRequiredService<IPrivateChatRepository>();
        var existingChat = new PrivateChat
        {
            Id = Guid.NewGuid(),
            CreatedAt = DateTime.Now,
            FirstUserId = authenticatedUser.Id,
            SecondUserId = messageReceiver.Id,
            Messages = []
        };
        await privateChatRepository.Insert(existingChat);

        var messageRepository = fixture.Services.GetRequiredService<IMessageRepository>();
        var existingMessage = new Message
        {
            Id = Guid.NewGuid(),
            Content = "test message to delete",
            CreatedAt = DateTime.Now,
            ChatId = existingChat.Id,
            CreatedById = authenticatedUser.Id
        };
        await messageRepository.Insert(existingMessage);

        // Act
        using var result = await client.DeleteAsync($"api/messages/{existingMessage.Id}");

        // Assert
        result.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    public Task InitializeAsync() => Task.CompletedTask;

    public async Task DisposeAsync() =>
        await _postgreFixture.ResetDatabase();
}
