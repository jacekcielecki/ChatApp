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

        const int pageSize = 5;
        const int pageNumber = 1;
        var request = new GetMessagesParamsDto(TestConfig.StudyGroupChat.Id, ChatType.Group, pageSize, pageNumber);

        var result = await getMessages.Get(request, TestConfig.LoggedUserId);

        Assert.Equal(typeof(Success<PagedResult<MessageDto>>), result.Value.GetType());
    }
}