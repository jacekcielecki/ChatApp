using ChatApp.Contracts.Request;
using ChatApp.Domain.Entities;
using ChatApp.Domain.ResultTypes;
using OneOf;
using OneOf.Types;

namespace ChatApp.Application.Interfaces;

public interface IMessageService
{
    Task<OneOf<Success, ValidationErrors>> CreateGroup(CreateGroupMessageRequest request, User user);
    Task<OneOf<Success, ValidationErrors>> CreatePrivate(CreatePrivateMessageRequest request, User user);
}
