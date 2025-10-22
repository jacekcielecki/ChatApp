using ChatApp.Chats.Commands;
using ChatApp.Shared.Model.Chats;
using ChatApp.Shared.Tests.Setup;
using OneOf.Types;

namespace ChatApp.Chats.Tests.Integration;

public class UpdateGroupChatIntegrationTests : IClassFixture<IntegrationTestFixture>
{
    private readonly IntegrationTestFixture _fixture;

    public UpdateGroupChatIntegrationTests(IntegrationTestFixture fixture)
    {
        _fixture = fixture;
    }

    [Fact]
    public async Task UpdateGroupChat_ReturnsSuccess()
    {
        var updateGroupChat = _fixture.ResolveService<UpdateGroupChat>();

        var createPrivateChatRequest = new GroupChatUpdateApiDto(
            TestConfig.ExistingGroupChat.Id,
            "Test Group Chat updated",
            [TestConfig.UserAlice.Id, TestConfig.UserCharlie.Id]);

        var result = await updateGroupChat.Update(createPrivateChatRequest, TestConfig.LoggedUserId);

        Assert.Equal(typeof(Success), result.Value.GetType());
    }
}