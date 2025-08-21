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
    public async Task GetChats_Should_Return_Ok_WithChats()
    {
        var getChats = _fixture.ResolveService<GetChats>();
        var existingUserId = Guid.Parse("98778b84-6108-45c0-b4b9-a7ac71059ce5");
        var existingChatId = Guid.Parse("4398407C-7AAB-40AA-A88A-618B7E4F5701");

        var result = await getChats.Get(existingUserId);

        Assert.NotNull(result);
        Assert.True(result.PrivateChats.Any(x => x.Id == existingChatId));
    }
}