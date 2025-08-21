using ChatApp.Shared.Tests.Setup;
using ChatApp.Users.Queries;

namespace ChatApp.Users.Tests.Integration;

public class GetUsersIntegrationTests : IClassFixture<IntegrationTestFixture>
{
    private readonly IntegrationTestFixture _fixture;

    public GetUsersIntegrationTests(IntegrationTestFixture fixture)
    {
        _fixture = fixture;
    }

    [Fact]
    public async Task GetUsers_ReturnsUser_ContainingSpecifiedSearchPhrase()
    {
        var getUsers = _fixture.ResolveService<GetUsersBySearchPhrase>();
        var searchPhrase = "example";

        var users = await getUsers.Get(searchPhrase);

        Assert.NotNull(users);
        Assert.True(users.All(x => x.Contains(searchPhrase)));
    }
}