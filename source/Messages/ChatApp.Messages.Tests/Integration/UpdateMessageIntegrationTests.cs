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

        var authenticatedUserId = Guid.Parse("a6e58f6e-fef3-458c-a26c-19cfc14d329e");
        var existingMessageId = Guid.Parse("989AB14F-9210-4A51-8EA6-256B553DA825");

        var request = new MessageUpdateApiDto(existingMessageId, "Message content updated");
        var result = await updateMessage.Update(request, authenticatedUserId);

        Assert.Equal(typeof(Success), result.Value.GetType());
    }
}
