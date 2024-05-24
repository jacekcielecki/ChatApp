﻿using ChatApp.Application.Interfaces;
using ChatApp.Contracts.Request;
using ChatApp.Domain.Entities;
using ChatApp.Domain.Interfaces.Repositories;
using ChatApp.Domain.ResultTypes;
using OneOf;
using OneOf.Types;

namespace ChatApp.Application.Services;

public class ChatService : IChatService
{
    private readonly IGroupChatRepository _groupChatRepository;
    private readonly IUserService _userService;

    public ChatService(IGroupChatRepository groupChatRepository, IUserService userService)
    {
        _groupChatRepository = groupChatRepository;
        _userService = userService;
    }

    public async Task<IEnumerable<GroupChat>> GetGroupChats(Guid userId)
    {
        return await _groupChatRepository.Get(userId);
    }

    public async Task<OneOf<Success<Guid>, ValidationErrors>> CreateGroup(CreateGroupChatRequest request, Guid creatorId)
    {
        Dictionary<string, string[]> validationErrors = new();
        if (request.Name.Length < 5)
        {
            validationErrors.Add("Name", ["Chat name has to be at least 5 character long"]);
        }

        var groupChat = new GroupChat
        {
            Id = Guid.NewGuid(),
            Name = request.Name,
            CreatedAt = DateTime.UtcNow,
            CreatedById = creatorId,
            Members = [],
            Messages = []
        };

        var members = request.Members.Append(creatorId).Distinct().ToList();
        foreach (var memberId in members)
        {
            var user = await _userService.GetById(memberId);
            if (user == null)
            {
                validationErrors.Add("ReceiverId", [$"Chat member with id {memberId} not found"]);
                break;
            }
            groupChat.Members.Add(user);
        }

        if (validationErrors.Any())
        {
            return new ValidationErrors(validationErrors);
        }

        await _groupChatRepository.Insert(groupChat);
        return new Success<Guid>(groupChat.Id);
    }
}