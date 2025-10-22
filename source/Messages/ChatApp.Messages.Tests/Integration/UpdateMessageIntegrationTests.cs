using ChatApp.Messages.Commands;
using ChatApp.Shared.Model.Messages;
using ChatApp.Shared.Tests.Setup;
using OneOf.Types;

namespace ChatApp.Messages.Tests.Integration;

public class UpdateMessageIntegrationTests : IClassFixture<IntegrationTestFixture>
{
    private readonly IntegrationTestFixture _fixture;

    public UpdateMessageIntegrationTests(IntegrationTestFixture fixture)
    {
        _fixture = fixture;
    }

    [Fact]
    public async Task UpdateMessage_Should_Return_Success()
    {
        var updateMessage = _fixture.ResolveService<UpdateMessage>();

        var authenticatedUserId = Guid.Parse("98778b84-6108-45c0-b4b9-a7ac71059ce5");
        var existingMessageId = Guid.Parse("989AB14F-9210-4A51-8EA6-256B553DA825");

        var request = new MessageUpdateApiDto(existingMessageId, "Message content updated");
        var result = await updateMessage.Update(request, authenticatedUserId);

        Assert.Equal(typeof(Success), result.Value.GetType());
    }
}
