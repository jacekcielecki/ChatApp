using System.Net;

namespace ChatApp.IntegrationTests.Endpoints;

public sealed class UserEndpointsTests
{
    [Fact]
    public async Task GettingUser_WhenUserWithGivenNameFound_ReturnsUser()
    {
        // Arrange
        var name = "Johny";
        var fixture = new UserEndpointsFixture();
        using var client = fixture.CreateClient();

        // Act
        using var result = await client.GetAsync($"api/users/{name}");

        // Assert
        result.StatusCode.Should().Be(HttpStatusCode.OK);
    }
}
