using ChatApp.IntegrationTests.Tools;
using System.Net;
using System.Net.Http.Json;

namespace ChatApp.IntegrationTests.Endpoints;

[Collection("PostgreSql collection")]
public sealed class UserEndpointsTests
{
    private readonly PostgreSqlFixture _postgreFixture;

    public UserEndpointsTests(PostgreSqlFixture postgreFixture)
    {
        _postgreFixture = postgreFixture;
    }

    [Fact]
    public async Task GettingCurrentUser_WhenAuthorized_ReturnsUser()
    {
        // Arrange
        var fixture = new TestApiFixture(_postgreFixture);
        using var client = fixture.CreateClient();

        // Act
        using var result = await client.GetAsync("api/users/me");

        // Assert
        result.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task GettingUserEmailsBySearchPhrase_WithExistingUsers_ReturnsUserEmailsMatchingSearchPhrase()
    {
        // Arrange
        var searchPhrase = "test";
        var fixture = new TestApiFixture(_postgreFixture);
        using var client = fixture.CreateClient();

        // Act
        using var result = await client.GetAsync($"api/users/search/{searchPhrase}");

        // Assert
        result.StatusCode.Should().Be(HttpStatusCode.OK);
        var content = await result.Content.ReadFromJsonAsync<string[]>();
        content.Should().NotBeNull();
    }
}
