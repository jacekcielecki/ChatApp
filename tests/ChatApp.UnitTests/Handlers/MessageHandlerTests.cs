using ChatApp.Application.Handlers;
using ChatApp.Contracts.Request;
using ChatApp.Domain.Entities;
using ChatApp.Domain.Interfaces.Repositories;
using OneOf.Types;

namespace ChatApp.UnitTests.Handlers;

public sealed class MessageHandlerTests
{
    private readonly IGroupChatRepository _groupChatRepositoryMock = Substitute.For<IGroupChatRepository>();
    private readonly IPrivateChatRepository _privateChatRepositoryMock = Substitute.For<IPrivateChatRepository>();
    private readonly IMessageRepository _messageRepositoryMock = Substitute.For<IMessageRepository>();
    private readonly MessageHandler _messageHandler;

    public MessageHandlerTests()
    {
        _messageHandler = new MessageHandler(_messageRepositoryMock, _groupChatRepositoryMock, _privateChatRepositoryMock);
    }

    [Fact]
    public async Task GettingPaged_WithValidRequest_ReturnsRequestedMessagesWithTotalMessagesCount()
    {
        // Arrange
        var user = new User { Id = Guid.NewGuid(), Email = "johny@mail.com", CreatedAt = DateTime.UtcNow };
        var existingChat = new GroupChat
        {
            Id = Guid.NewGuid(),
            Name = "Test chat",
            CreatedAt = DateTime.UtcNow,
            CreatedById = user.Id,
            Members = [user],
            Messages = [ 
                new Message { Id = Guid.NewGuid(), ChatId = Guid.NewGuid(), Content = "Test msg 1", CreatedAt = DateTime.Now, CreatedById = user.Id }, 
                new Message { Id = Guid.NewGuid(), ChatId = Guid.NewGuid(), Content = "Test msg 1", CreatedAt = DateTime.Now, CreatedById = user.Id }
            ]
        };
        _groupChatRepositoryMock.GetById(existingChat.Id).Returns(existingChat);

        var request = new GetPagedMessagesRequest(ChatId: existingChat.Id, PageNumber: 1, PageSize: 10);

        // Act
        var result = await _messageHandler.GetPaged(request, user);

        // Assert
        result.Value.Should().BeOfType<Success<(IEnumerable<Message>, int)>>();
    }

    [Fact]
    public async Task CreateGroupChatMessage_WithValidRequest_ReturnsSuccess()
    {
        // Arrange
        var user = new User { Id = Guid.NewGuid(), Email = "johny@mail.com", CreatedAt = DateTime.UtcNow };

        var existingChat = new GroupChat
        {
            Id = Guid.NewGuid(),
            Name = "Test chat",
            CreatedAt = DateTime.UtcNow,
            CreatedById = user.Id,
            Members = [user],
            Messages = [
                new Message { Id = Guid.NewGuid(), ChatId = Guid.NewGuid(), Content = "Test msg 1", CreatedAt = DateTime.Now, CreatedById = user.Id },
                new Message { Id = Guid.NewGuid(), ChatId = Guid.NewGuid(), Content = "Test msg 1", CreatedAt = DateTime.Now, CreatedById = user.Id }
            ]
        };
        _groupChatRepositoryMock.GetById(existingChat.Id).Returns(existingChat);

        var request = new CreateGroupMessageRequest(existingChat.Id, "hello guys");

        // Act
        var result = await _messageHandler.CreateGroup(request, user);

        // Assert
        result.Value.Should().BeOfType<Success>();
    }

    [Fact]
    public async Task CreatePrivateChatMessage_WithValidRequest_ReturnsSuccess()
    {
        // Arrange
        var sender = new User { Id = Guid.NewGuid(), Email = "johny@mail.com", CreatedAt = DateTime.UtcNow };
        var receiver = new User { Id = Guid.NewGuid(), Email = "mike@mail.com", CreatedAt = DateTime.UtcNow };

        var existingChat = new PrivateChat
        {
            Id = Guid.NewGuid(),
            CreatedAt = DateTime.UtcNow,
            FirstUserId = sender.Id,
            SecondUserId = receiver.Id,
            Receiver = receiver,
            Messages = []
        };
        _privateChatRepositoryMock.GetById(existingChat.Id).Returns(existingChat);

        var request = new CreatePrivateMessageRequest(existingChat.Id, "Hi Mike");

        // Act
        var result = await _messageHandler.CreatePrivate(request, sender);

        // Assert
        result.Value.Should().BeOfType<Success>();
    }

    [Fact]
    public async Task UpdateMessage_WithValidRequest_ReturnsSuccess()
    {
        // Arrange
        var sender = new User { Id = Guid.NewGuid(), Email = "johny@mail.com", CreatedAt = DateTime.UtcNow };

        var messageToUpdate = new Message
        {
            Id = Guid.NewGuid(),
            ChatId = Guid.NewGuid(),
            Content = "Hi Mike",
            CreatedAt = DateTime.Now,
            CreatedById = sender.Id
        };
        _messageRepositoryMock.GetById(messageToUpdate.Id).Returns(messageToUpdate);

        var request = new UpdateMessageRequest(messageToUpdate.Id, "Hi Mike, this message was updated!");

        // Act
        var result = await _messageHandler.Update(request, sender);

        // Assert
        result.Value.Should().BeOfType<Success>();
    }

    [Fact]
    public async Task DeleteMessage_WithValidRequest_ReturnsSuccess()
    {
        // Arrange
        var sender = new User { Id = Guid.NewGuid(), Email = "johny@mail.com", CreatedAt = DateTime.UtcNow };
        
        var messageToDelete = new Message
        {
            Id = Guid.NewGuid(),
            ChatId = Guid.NewGuid(),
            Content = "Hi Mike",
            CreatedAt = DateTime.Now,
            CreatedById = sender.Id
        };
        _messageRepositoryMock.GetById(messageToDelete.Id).Returns(messageToDelete);

        // Act
        var result = await _messageHandler.Delete(messageToDelete.Id, sender);

        // Assert
        result.Value.Should().BeOfType<Success>();
    }
}