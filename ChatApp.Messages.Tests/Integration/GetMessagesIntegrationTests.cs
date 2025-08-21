using ChatApp.Messages.Queries;
using ChatApp.Shared.Model.Chats;
using ChatApp.Shared.Model.Messages;
using ChatApp.Shared.Model.ValueObjects;
using ChatApp.Shared.Tests.Setup;
using OneOf.Types;

namespace ChatApp.Messages.Tests.Integration;

public class GetMessagesIntegrationTests : IClassFixture<IntegrationTestFixture>
{
    private readonly IntegrationTestFixture _fixture;

    public GetMessagesIntegrationTests(IntegrationTestFixture fixture)
    {
        _fixture = fixture;
    }

    [Fact]
    public async Task GetMessages_Should_Return_Success_WithMessages()
    {
        var getMessages = _fixture.ResolveService<GetMessages>();

        var existingGroupChatId = Guid.Parse("C653AF32-7980-4480-ACF2-708A0653ECBA");
        var existingUserId = Guid.Parse("a6e58f6e-fef3-458c-a26c-19cfc14d329e");

        const int pageSize = 5;
        const int pageNumber = 1;
        var request = new GetMessagesRequest(existingGroupChatId, ChatType.Group, pageSize, pageNumber);

        var result = await getMessages.Get(request, existingUserId);

        Assert.Equal(typeof(Success<PagedResult<MessageResponse>>), result.Value.GetType());
    }
}