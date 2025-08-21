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

        var existingUserId = Guid.Parse("98778b84-6108-45c0-b4b9-a7ac71059ce5");
        var existingUser2Id = Guid.Parse("ff32d4d0-86ca-41be-9157-9a2ce3d5bcd4");
        var createGroupChatRequest = new CreateGroupChatRequest("Test group chat", [existingUserId, existingUser2Id]);

        var result = await createGroupChat.Create(createGroupChatRequest, existingUserId);

        Assert.Equal(result.Value.GetType(), typeof(Success<Guid?>));
    }
}
