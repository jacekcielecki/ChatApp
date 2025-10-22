using ChatApp.Chats.Commands;
using ChatApp.Shared.Model.Chats;
using ChatApp.Shared.Tests.Setup;
using OneOf.Types;

namespace ChatApp.Chats.Tests.Integration;

public class CreateGroupChatIntegrationTests : IClassFixture<IntegrationTestFixture>
{
    private readonly IntegrationTestFixture _fixture;

    public CreateGroupChatIntegrationTests(IntegrationTestFixture fixture)
    {
        _fixture = fixture;
    }

    [Fact]
    public async Task CreateGroupChat_ReturnsSuccess()
    {
        var createGroupChat = _fixture.ResolveService<CreateGroupChat>();

        var createGroupChatRequest = new GroupChatCreateApiDto("Test group chat", [TestConfig.UserAlice.Id]);

        var result = await createGroupChat.Create(createGroupChatRequest, TestConfig.LoggedUserId);

        Assert.Equal(typeof(Success<Guid?>), result.Value.GetType());
    }
}
