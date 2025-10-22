using ChatApp.Messages.Commands;
using ChatApp.Shared.Model.Messages;
using ChatApp.Shared.Tests.Setup;
using OneOf.Types;

namespace ChatApp.Messages.Tests.Integration;

public class CreateGroupChatMessageIntegrationTests : IClassFixture<IntegrationTestFixture>
{
    private readonly IntegrationTestFixture _fixture;

    public CreateGroupChatMessageIntegrationTests(IntegrationTestFixture fixture)
    {
        _fixture = fixture;
    }

    [Fact]
    public async Task CreateGroupChatMessage_Should_Return_Success()
    {
        var createGroupChatMessage = _fixture.ResolveService<CreateGroupChatMessage>();

        var request = new MessageCreateApiDto { ChatId = TestConfig.StudyGroupChat.Id, Content = "Hi Guys!" };

        var result = await createGroupChatMessage.Create(request, TestConfig.LoggedUserId);

        Assert.Equal(typeof(Success<Guid?>), result.Value.GetType());
    }
}
