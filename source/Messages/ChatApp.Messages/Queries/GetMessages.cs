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

    public async Task<OneOf<Success<PagedResult<MessageDto>>, NotFound, Forbidden, ValidationErrors>> Get(GetMessagesParamsDto paramsDto, Guid userId)
    {
        var validationErrors = ValidateRequest(paramsDto);
        if (validationErrors.Any())
        {
            return new ValidationErrors(validationErrors);
        }

        if (paramsDto.ChatType is ChatType.Private)
        {
            var privateChat = await _getPrivateChatByIdRepository.Get(paramsDto.ChatId);
            if (privateChat is null)
            {
                return new NotFound();
            }
            if (privateChat.FirstUserId != userId && privateChat.SecondUserId != userId)
            {
                return new Forbidden();
            }
        }

        if (paramsDto.ChatType is ChatType.Group)
        {
            var groupChat = await _getGroupChatByIdRepository.Get(paramsDto.ChatId);
            if (groupChat is null)
            {
                return new NotFound();
            }
            if (groupChat.Members.All(x => x.Id != userId) && groupChat.CreatedById != userId)
            {
                return new Forbidden();
            }
        }

        var skip = paramsDto.PageSize * (paramsDto.PageNumber - 1);
        var take = paramsDto.PageSize;

        var messages = await _getMessagesRepository.Get(paramsDto.ChatId, skip, take);

        var messagesPaged = new PagedResult<MessageDto>(
            messages.Items.Select(x => x.ToDto()),
            messages.TotalMessagesCount,
            paramsDto.PageSize,
            paramsDto.PageNumber);

        var result = new Success<PagedResult<MessageDto>>(messagesPaged);
        return result;
    }

    private Dictionary<string, string[]> ValidateRequest(GetMessagesParamsDto paramsDto)
    {
        var validationErrors = new Dictionary<string, string[]>();

        const int maxPageSize = 200;
        if (paramsDto.PageSize > maxPageSize)
        {
            validationErrors.Add(nameof(GetMessagesParamsDto.PageSize), ["Max page size is equal 200 messages"]);
        }

        return validationErrors;
    }
}
