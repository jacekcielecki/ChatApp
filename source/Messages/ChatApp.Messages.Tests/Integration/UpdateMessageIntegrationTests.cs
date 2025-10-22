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

        var request = new MessageUpdateApiDto(TestConfig.StudyGroupChat.DavidMessageId, "Message content updated");
        var result = await updateMessage.Update(request, TestConfig.LoggedUserId);

        Assert.Equal(typeof(Success), result.Value.GetType());
    }
}
