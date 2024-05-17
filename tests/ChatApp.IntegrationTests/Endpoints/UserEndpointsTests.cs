using ChatApp.IntegrationTests.Tools;
using System.Net;

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
        var fixture = new UserEndpointsFixture(_postgreFixture);
        using var client = fixture.CreateClient();

        // Act
        using var result = await client.GetAsync("api/users/me");

        // Assert
        result.StatusCode.Should().Be(HttpStatusCode.OK);
    }
}
