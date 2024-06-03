using ChatApp.Application.Handlers;
using ChatApp.Contracts.Request;
using ChatApp.Domain.Entities;
using ChatApp.Domain.Interfaces.Repositories;

namespace ChatApp.UnitTests.Handlers;

public sealed class UserHandlerTests
{
    private readonly IUserRepository _userRepositoryMock = Substitute.For<IUserRepository>();
    private readonly UserHandler _userHandler;

    public UserHandlerTests()
    {
        _userHandler = new UserHandler(_userRepositoryMock);
    }

    [Fact]
    public async Task GettingUserById_WhenUserWithGivenIdExists_ReturnsUser()
    {
        // Arrange
        var user = new User {Id = Guid.NewGuid(), Email = "johny@mail.com", CreatedAt = DateTime.UtcNow,};
        _userRepositoryMock.GetById(user.Id).Returns(user);

        // Act
        var result = await _userHandler.GetById(user.Id);

        // Assert
        result.Should().Be(user);
    }

    [Fact]
    public async Task GettingUserByEmail_WhenUserWithGivenEmailExists_ReturnsUser()
    {
        // Arrange
        var user = new User {Id = Guid.NewGuid(), Email = "johny@mail.com", CreatedAt = DateTime.UtcNow,};
        _userRepositoryMock.GetByEmail(user.Email).Returns(user);

        // Act
        var result = await _userHandler.GetByEmail(user.Email);

        // Assert
        result.Should().Be(user);
    }

    [Fact]
    public async Task GettingEmailsBySearchPhrase_WhenEmailsMatchingSearchPhraseExist_ReturnsEmails()
    {
        // Arrange
        var searchPhrase = "johny";
        string[] matchingResults = ["johny@mail.com", "johny2@mail.com", "johny3@mail.com"];
        _userRepositoryMock.GetEmailsBySearchPhrase(searchPhrase).Returns(matchingResults);

        // Act
        var result = await _userHandler.GetEmailsBySearchPhrase(searchPhrase);

        // Assert
        result.Should().Contain(matchingResults);
    }

    [Fact]
    public async Task Creating_WithValidCreateUserRequest_ReturnsCreatedUserId()
    {
        // Arrange
        var request = new CreateUserRequest("johny@mail.com");

        // Act
        var result = await _userHandler.Create(request);

        // Assert
        result.Should().HaveValue();
    }
}