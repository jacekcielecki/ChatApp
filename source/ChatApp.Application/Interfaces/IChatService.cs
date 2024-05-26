using ChatApp.Contracts.Request;
using ChatApp.Domain.Entities;
using ChatApp.Domain.ResultTypes;
using OneOf;
using OneOf.Types;

namespace ChatApp.Application.Interfaces;

public interface IChatService
{
    Task<IEnumerable<GroupChat>> GetGroupChats(Guid userId);
    Task<IEnumerable<PrivateChat>> GetPrivateChats(Guid userId);
    Task<OneOf<Success<Guid>, ValidationErrors>> CreateGroup(CreateGroupChatRequest request, Guid creatorId);
    Task<OneOf<Success<Guid>, ValidationErrors>> CreatePrivate(CreatePrivateChatRequest request, Guid creatorId);
}
