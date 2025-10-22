using ChatApp.Messages.Commands;
using ChatApp.Shared.Model.Messages;
using ChatApp.Shared.Tests.Setup;
using OneOf.Types;

namespace ChatApp.Messages.Tests.Integration;

public class CreatePrivateChatMessageIntegrationTests : IClassFixture<IntegrationTestFixture>
{
    private readonly IntegrationTestFixture _fixture;

    public CreatePrivateChatMessageIntegrationTests(IntegrationTestFixture fixture)
    {
        _fixture = fixture;
    }

    [Fact]
    public async Task CreatePrivateChatMessage_Should_Return_Success()
    {
        var createPrivateChatMessage = _fixture.ResolveService<CreatePrivateChatMessage>();

        var request = new MessageCreateApiDto { ChatId = TestConfig.AlicePrivateChat.Id, Content = "Hi, how are you?" };

        var result = await createPrivateChatMessage.Create(request, TestConfig.LoggedUserId);

        Assert.Equal(typeof(Success<Guid?>), result.Value.GetType());
    }
}
