using ChatApp.IntegrationTests.Tools;
using System.Net;

namespace ChatApp.IntegrationTests.Endpoints;

[Collection("PostgreSql collection")]
public sealed class VersionEndpointsTests
{
    private readonly PostgreSqlFixture _postgreFixture;

    public VersionEndpointsTests(PostgreSqlFixture postgreFixture)
    {
        _postgreFixture = postgreFixture;
    }

    [Fact]
    public async Task GettingVersion_WhenCalled_ReturnsVersion()
    {
        // Arrange
        var fixture = new TestApiFixture(_postgreFixture);
        using var client = fixture.CreateClient();

        // Act
        using var result = await client.GetAsync("api/version");

        // Assert
        result.StatusCode.Should().Be(HttpStatusCode.OK);
        var content = await result.Content.ReadAsStringAsync();
        content.Should().Be("1.0.0");
    }
}
