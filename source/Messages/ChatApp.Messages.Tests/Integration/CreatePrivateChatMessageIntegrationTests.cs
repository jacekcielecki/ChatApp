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

        var authenticatedUserId = Guid.Parse("98778b84-6108-45c0-b4b9-a7ac71059ce5");
        var existingPrivateChatId = Guid.Parse("4398407C-7AAB-40AA-A88A-618B7E4F5701");
        var messageContent = "Hi, how are you?";

        var request = new PrivateChatCreateApiDto(existingPrivateChatId, messageContent);

        var result = await createPrivateChatMessage.Create(request, authenticatedUserId);

        Assert.Equal(typeof(Success<Guid?>), result.Value.GetType());
    }
}
