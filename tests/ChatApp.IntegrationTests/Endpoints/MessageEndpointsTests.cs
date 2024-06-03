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

    public Task InitializeAsync() => Task.CompletedTask;

    public async Task DisposeAsync() =>
        await _postgreFixture.ResetDatabase();
}
