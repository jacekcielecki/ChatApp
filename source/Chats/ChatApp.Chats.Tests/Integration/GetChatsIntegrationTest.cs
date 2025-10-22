using ChatApp.Chats.Queries;
using ChatApp.Shared.Tests.Setup;

namespace ChatApp.Chats.Tests.Integration;

public class GetChatsIntegrationTest : IClassFixture<IntegrationTestFixture>
{
    private readonly IntegrationTestFixture _fixture;

    public GetChatsIntegrationTest(IntegrationTestFixture fixture)
    {
        _fixture = fixture;
    }

    [Fact]
    public async Task GetChats_Should_Return_Success_WithChats()
    {
        var getChats = _fixture.ResolveService<GetChats>();

        var result = await getChats.Get(TestConfig.LoggedUserId);

        Assert.NotNull(result);
        Assert.Contains(result, x => x.Id == TestConfig.ExistingPrivateChat.Id);
        Assert.Contains(result, x => x.Id == TestConfig.ExistingGroupChat.Id);
    }
}