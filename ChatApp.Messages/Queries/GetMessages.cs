using ChatApp.Chats.Core.Group;
using ChatApp.Chats.Core.Private;
using ChatApp.Messages.Core.Details;
using ChatApp.Messages.Core.Summary;
using ChatApp.Shared.Model.Chats;
using ChatApp.Shared.Model.Messages;
using ChatApp.Shared.Model.ValueObjects;
using OneOf;
using OneOf.Types;

namespace ChatApp.Messages.Queries;

public class GetMessages
{
    private readonly GetGroupChatByIdRepository _getGroupChatByIdRepository;
    private readonly GetPrivateChatByIdRepository _getPrivateChatByIdRepository;
    private readonly GetMessagesRepository _getMessagesRepository;

    public GetMessages(
        GetGroupChatByIdRepository getGroupChatByIdRepository,
        GetPrivateChatByIdRepository getPrivateChatByIdRepository,
        GetMessagesRepository getMessagesRepository)
    {
        _getGroupChatByIdRepository = getGroupChatByIdRepository;
        _getPrivateChatByIdRepository = getPrivateChatByIdRepository;
        _getMessagesRepository = getMessagesRepository;
    }

    public async Task<OneOf<Success<PagedResult<MessageResponse>>, NotFound, Forbidden, ValidationErrors>> Get(GetMessagesRequest request, Guid userId)
    {
        var validationErrors = ValidateRequest(request);
        if (validationErrors.Any())
        {
            return new ValidationErrors(validationErrors);
        }

        if (request.ChatType is ChatType.Private)
        {
            var privateChat = await _getPrivateChatByIdRepository.Get(request.ChatId);
            if (privateChat is null)
            {
                return new NotFound();
            }
            if (privateChat.FirstUserId != userId && privateChat.SecondUserId != userId)
            {
                return new Forbidden();
            }
        }

        if (request.ChatType is ChatType.Group)
        {
            var groupChat = await _getGroupChatByIdRepository.Get(request.ChatId);
            if (groupChat is null)
            {
                return new NotFound();
            }
            if (groupChat.Members.All(x => x.Id != userId))
            {
                return new Forbidden();
            }
        }

        var skip = request.PageSize * (request.PageNumber - 1);
        var take = request.PageSize;

        var messages = await _getMessagesRepository.Get(request.ChatId, skip, take);

        var messagesPaged = new PagedResult<MessageResponse>(
            messages.Items.Select(x => x.ToResponse()),
            messages.TotalMessagesCount,
            request.PageSize,
            request.PageNumber);

        var result = new Success<PagedResult<MessageResponse>>(messagesPaged);
        return result;
    }

    private Dictionary<string, string[]> ValidateRequest(GetMessagesRequest request)
    {
        var validationErrors = new Dictionary<string, string[]>();

        const int maxPageSize = 200;
        if (request.PageSize > maxPageSize)
        {
            validationErrors.Add(nameof(GetMessagesRequest.PageSize), ["Max page size is equal 200 messages"]);
        }

        return validationErrors;
    }
}
