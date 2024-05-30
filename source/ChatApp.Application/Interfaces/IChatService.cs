using ChatApp.Contracts.Request;
using ChatApp.Domain.Entities;
using ChatApp.Domain.ResultTypes;
using OneOf;
using OneOf.Types;

namespace ChatApp.Application.Interfaces;

public interface IChatService
{
    Task<IEnumerable<GroupChat>> GetGroupChats(User user);
    Task<IEnumerable<PrivateChat>> GetPrivateChats(User user);
    Task<OneOf<Success<Guid>, ValidationErrors>> CreateGroup(CreateGroupChatRequest request, User user);
    Task<OneOf<Success<Guid>, ValidationErrors>> CreatePrivate(CreatePrivateChatRequest request, User user);
    Task<OneOf<Success, NotFound, Forbidden, ValidationErrors>> UpdateGroup(UpdateGroupChatRequest request, User user);
}
