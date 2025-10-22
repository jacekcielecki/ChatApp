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

        var existingGroupChatId = Guid.Parse("C653AF32-7980-4480-ACF2-708A0653ECBA");
        var existingUserId = Guid.Parse("a6e58f6e-fef3-458c-a26c-19cfc14d329e");
        var existingUser2Id = Guid.Parse("ff32d4d0-86ca-41be-9157-9a2ce3d5bcd4");

        var createPrivateChatRequest = new GroupChatUpdateApiDto(existingGroupChatId, "Test Group Chat updated", [existingUserId, existingUser2Id]);

        var result = await updateGroupChat.Update(createPrivateChatRequest, existingUserId);

        Assert.Equal(typeof(Success), result.Value.GetType());
    }
}