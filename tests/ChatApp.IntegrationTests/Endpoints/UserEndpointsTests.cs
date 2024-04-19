using System.Net;

namespace ChatApp.IntegrationTests.Endpoints;

public sealed class UserEndpointsTests
{
    [Fact]
    public async Task GettingUser_WhenUserWithGivenEmailFound_ReturnsUser()
    {
        // Arrange
        var email = "johny@mail.com";
        var fixture = new UserEndpointsFixture();
        using var client = fixture.CreateClient();

        // Act
        using var result = await client.GetAsync($"api/users/{email}");

        // Assert
        result.StatusCode.Should().Be(HttpStatusCode.OK);
    }
}
