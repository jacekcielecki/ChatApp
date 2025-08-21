using ChatApp.Chats.Commands;
using ChatApp.Shared.Model.Chats;
using ChatApp.Shared.Tests.Setup;
using OneOf.Types;

namespace ChatApp.Chats.Tests.Integration;

public class CreatePrivateChatIntegrationTests : IClassFixture<IntegrationTestFixture>
{
    private readonly IntegrationTestFixture _fixture;

    public CreatePrivateChatIntegrationTests(IntegrationTestFixture fixture)
    {
        _fixture = fixture;
    }

    [Fact]
    public async Task CreatePrivateChat_ReturnsSuccess()
    {
        var createPrivateChat = _fixture.ResolveService<CreatePrivateChat>();

        var authenticatedUserId = Guid.Parse("98778b84-6108-45c0-b4b9-a7ac71059ce5");
        var existingUserId = Guid.Parse("ff32d4d0-86ca-41be-9157-9a2ce3d5bcd4");
        var createPrivateChatRequest = new CreatePrivateChatRequest(existingUserId);

        var result = await createPrivateChat.Create(createPrivateChatRequest, authenticatedUserId);

        Assert.Equal(typeof(Success<Guid?>), result.Value.GetType());
    }
}
