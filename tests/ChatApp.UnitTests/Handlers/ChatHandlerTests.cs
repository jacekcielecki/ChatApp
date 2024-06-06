using ChatApp.Application.Handlers;
using ChatApp.Contracts.Request;
using ChatApp.Domain.Entities;
using ChatApp.Domain.Interfaces.Repositories;
using OneOf.Types;

namespace ChatApp.UnitTests.Handlers;

public sealed class ChatHandlerTests
{
    private readonly IGroupChatRepository _groupChatRepository = Substitute.For<IGroupChatRepository>();
    private readonly IPrivateChatRepository _privateChatRepository = Substitute.For<IPrivateChatRepository>();
    private readonly IUserRepository _userRepositoryMock = Substitute.For<IUserRepository>();
    private readonly ChatHandler _chatHandler;

    public ChatHandlerTests()
    {
        _chatHandler = new ChatHandler(_groupChatRepository, _privateChatRepository, _userRepositoryMock);
    }

    [Fact]
    public async Task GettingGroupChats_WhenItemsWhereUserIsMemberExist_ReturnsGroupChats()
    {
        // Arrange
        var user = new User { Id = Guid.NewGuid(), Email = "johny@mail.com", CreatedAt = DateTime.UtcNow, };
        var expectedResult = new List<GroupChat>
        {
            new() { Id = Guid.NewGuid(), Name = "Group chat 1", CreatedAt = DateTime.UtcNow, CreatedById = user.Id, Members = [user], Messages = [] },
            new() { Id = Guid.NewGuid(), Name = "Group chat 2", CreatedAt = DateTime.UtcNow, CreatedById = user.Id, Members = [user], Messages = [] }
        };
        _groupChatRepository.Get(user.Id).Returns(expectedResult);

        // Act
        var result = await _chatHandler.GetGroupChats(user);

        // Assert
        result.Should().Equal(expectedResult);
    }

    [Fact]
    public async Task GettingPrivateChats_WhenItemsWhereUserIsFirstUserOrSecondUserExist_ReturnsGroupChats()
    {
        // Arrange
        var user = new User { Id = Guid.NewGuid(), Email = "johny@mail.com", CreatedAt = DateTime.UtcNow, };
        var expectedResult = new List<PrivateChat>
        {
            new() { Id = Guid.NewGuid(), CreatedAt = DateTime.UtcNow, FirstUserId = user.Id, SecondUserId = Guid.NewGuid(), Messages = [] },
            new() { Id = Guid.NewGuid(), CreatedAt = DateTime.UtcNow, FirstUserId = user.Id, SecondUserId = Guid.NewGuid(), Messages = [] },
        };
        _privateChatRepository.Get(user.Id).Returns(expectedResult);

        // Act
        var result = await _chatHandler.GetPrivateChats(user);

        // Assert
        result.Should().Equal(expectedResult);
    }

    [Fact]
    public async Task CreatingGroupChat_WithValidCreateRequest_ReturnsCreatedChatId()
    {
        // Arrange
        var user = new User { Id = Guid.NewGuid(), Email = "johny@mail.com", CreatedAt = DateTime.UtcNow, };
        var request = new CreateGroupChatRequest(Name: "Group chat 1", Members: []);

        _userRepositoryMock.GetById(user.Id).Returns(user);

        // Act
        var result = await _chatHandler.CreateGroup(request, user);

        // Assert
        result.Value.Should().BeOfType<Success<Guid>>();
    }

    [Fact]
    public async Task CreatingPrivateChat_WithValidCreateRequest_ReturnsCreatedChatId()
    {
        // Arrange
        var user = new User { Id = Guid.NewGuid(), Email = "johny@mail.com", CreatedAt = DateTime.UtcNow, };
        var receiver = new User { Id = Guid.NewGuid(), Email = "mike@mail.com", CreatedAt = DateTime.UtcNow, };
        var request = new CreatePrivateChatRequest(ReceiverId: receiver.Id);

        _userRepositoryMock.GetById(user.Id).Returns(user);
        _userRepositoryMock.GetById(receiver.Id).Returns(receiver);

        // Act
        var result = await _chatHandler.CreatePrivate(request, user);

        // Assert
        result.Value.Should().BeOfType<Success<Guid>>();
    }

    [Fact]
    public async Task UpdatingGroupChat_WithValidUpdateRequest_ReturnsSuccess()
    {
        // Arrange
        var user = new User { Id = Guid.NewGuid(), Email = "johny@mail.com", CreatedAt = DateTime.UtcNow, };
        var member1 = new User { Id = Guid.NewGuid(), Email = "mike@mail.com", CreatedAt = DateTime.UtcNow, };
        var member2 = new User { Id = Guid.NewGuid(), Email = "paul@mail.com", CreatedAt = DateTime.UtcNow, };
        var existingGroupChat = new GroupChat
        {
            Id = Guid.NewGuid(),
            Name = "Group chat 1",
            CreatedAt = DateTime.UtcNow,
            CreatedById = user.Id,
            Members = [user, member1],
            Messages = []
        };

        var request = new UpdateGroupChatRequest(Id: existingGroupChat.Id, Name: "Group chat 1 after update", Members: [user.Id, member2.Id]);

        _userRepositoryMock.GetById(user.Id).Returns(user);
        _userRepositoryMock.GetById(member1.Id).Returns(member1);
        _userRepositoryMock.GetById(member2.Id).Returns(member2);
        _groupChatRepository.GetById(existingGroupChat.Id).Returns(existingGroupChat);

        // Act
        var result = await _chatHandler.UpdateGroup(request, user);

        // Assert
        result.Value.Should().BeOfType<Success>();
    }
}