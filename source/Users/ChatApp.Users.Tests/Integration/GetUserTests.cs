using ChatApp.Shared.Tests.Setup;
using ChatApp.Users.Queries;

namespace ChatApp.Users.Tests.Integration;

public class GetUserTests : IClassFixture<IntegrationTestFixture>
{
    private readonly IntegrationTestFixture _fixture;

    public GetUserTests(IntegrationTestFixture fixture)
    {
        _fixture = fixture;
    }

    [Fact]
    public async Task GetUser_ReturnsUser_ByEmailClaim()
    {
        var getUser = _fixture.ResolveService<GetUser>();
        var authenticatedUserId = Guid.Parse("98778b84-6108-45c0-b4b9-a7ac71059ce5");

        var user = await getUser.Get();

        Assert.NotNull(user);
        Assert.True(user.Id == authenticatedUserId);
    }
}
