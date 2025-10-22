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

        var existingGroupChatId = Guid.Parse("C653AF32-7980-4480-ACF2-708A0653ECBA");
        var existingUserId = Guid.Parse("a6e58f6e-fef3-458c-a26c-19cfc14d329e");
        var messageContent = "Hi Guys!";
        
        var request = new GroupChatMessageCreateApiDto(existingGroupChatId, messageContent);

        var result = await createGroupChatMessage.Create(request, existingUserId);

        Assert.Equal(typeof(Success<Guid?>), result.Value.GetType());
    }
}
