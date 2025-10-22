using ChatApp.Messages.Commands;
using ChatApp.Shared.Model.Messages;
using ChatApp.Shared.Tests.Setup;
using OneOf.Types;

namespace ChatApp.Messages.Tests.Integration;

public class DeleteMessageIntegrationTests : IClassFixture<IntegrationTestFixture>
{
    private readonly IntegrationTestFixture _fixture;

    public DeleteMessageIntegrationTests(IntegrationTestFixture fixture)
    {
        _fixture = fixture;
    }

    [Fact]
    public async Task DeleteMessage_Should_Return_Success()
    {
        var deleteMessage = _fixture.ResolveService<DeleteMessageById>();

        var result = await deleteMessage.Delete(TestConfig.StudyGroupChat.DavidMessageId, TestConfig.LoggedUserId);

        Assert.Equal(typeof(Success), result.Value.GetType());
    }
}
