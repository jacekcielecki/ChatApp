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

        var createPrivateChatRequest = new PrivateChatCreateApiDto(TestConfig.UserAlice.Id);

        var result = await createPrivateChat.Create(createPrivateChatRequest, TestConfig.LoggedUserId);

        Assert.Equal(typeof(Success<Guid?>), result.Value.GetType());
    }
}
