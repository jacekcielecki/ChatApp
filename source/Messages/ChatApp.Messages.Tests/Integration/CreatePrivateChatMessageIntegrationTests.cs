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

        var authenticatedUserId = Guid.Parse("a6e58f6e-fef3-458c-a26c-19cfc14d329e");
        var existingPrivateChatId = Guid.Parse("4398407C-7AAB-40AA-A88A-618B7E4F5701");
        var messageContent = "Hi, how are you?";

        var request = new MessageCreateApiDto { ChatId = existingPrivateChatId, Content = messageContent };

        var result = await createPrivateChatMessage.Create(request, authenticatedUserId);

        Assert.Equal(typeof(Success<Guid?>), result.Value.GetType());
    }
}
