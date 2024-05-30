﻿using ChatApp.Contracts.Request;
using ChatApp.Domain.Entities;
using ChatApp.Domain.ResultTypes;
using OneOf;
using OneOf.Types;

namespace ChatApp.Application.Interfaces;

public interface IMessageService
{
    Task<OneOf<Success, NotFound, ValidationErrors>> CreateGroup(CreateGroupMessageRequest request, User user);
    Task<OneOf<Success, NotFound, ValidationErrors>> CreatePrivate(CreatePrivateMessageRequest request, User user);
    Task<OneOf<Success, NotFound, Forbidden, ValidationErrors>> Update(UpdateMessageRequest request, User user);
    Task<OneOf<Success, NotFound, Forbidden>> Delete(Guid id, User user);
}
