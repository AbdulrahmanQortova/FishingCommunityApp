using FishingCommunity.Application.Common.Interfaces;
using FishingCommunity.Application.Common.Models;
using FishingCommunity.Application.Features.Auth.Commands.Register;
using FishingCommunity.Shared.Wrappers;
using FluentAssertions;
using MediatR;
using Moq;
using Xunit;

namespace FishingCommunity.UnitTests.Application.Auth;

public class RegisterCommandHandlerTests
{
    private readonly Mock<IIdentityService> _identityServiceMock = new();
    private readonly Mock<IEmailService> _emailServiceMock = new();
    private readonly Mock<IPublisher> _publisherMock = new();
    private readonly RegisterCommandHandler _handler;

    public RegisterCommandHandlerTests()
    {
        _handler = new RegisterCommandHandler(_identityServiceMock.Object, _emailServiceMock.Object, _publisherMock.Object );
    }

    private static RegisterCommand CreateValidCommand() => new()
    {
        FirstName = "Ahmed",
        LastName = "Mostafa",
        Email = "ahmed@example.com",
        Password = "Pass@word1",
        ConfirmPassword = "Pass@word1",
        Role = "RegularUser"
    };

    [Fact]
    public async Task Handle_WhenUserCreationSucceeds_ReturnsSuccessResult()
    {
        // Arrange
        var command = CreateValidCommand();
        var newUserId = Guid.NewGuid();

        _identityServiceMock
            .Setup(s => s.CreateUserAsync(command.Email, command.Password, command.FirstName, command.LastName, command.Role, It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result<Guid>.Success(newUserId));

        _identityServiceMock
            .Setup(s => s.GenerateNewEmailVerificationCodeAsync(newUserId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result<string>.Success("123456"));

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Succeeded.Should().BeTrue();
        result.Data!.UserId.Should().Be(newUserId);
        result.Data.Email.Should().Be(command.Email);
    }

    [Fact]
    public async Task Handle_WhenUserCreationSucceeds_SendsVerificationEmail()
    {
        // Arrange
        var command = CreateValidCommand();
        var newUserId = Guid.NewGuid();

        _identityServiceMock
            .Setup(s => s.CreateUserAsync(command.Email, command.Password, command.FirstName, command.LastName, command.Role, It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result<Guid>.Success(newUserId));

        _identityServiceMock
            .Setup(s => s.GenerateNewEmailVerificationCodeAsync(newUserId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result<string>.Success("654321"));

        // Act
        await _handler.Handle(command, CancellationToken.None);

        // Assert
        _emailServiceMock.Verify(
            e => e.SendEmailVerificationAsync(command.Email, command.FirstName, "654321", It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task Handle_WhenEmailAlreadyExists_ReturnsFailureResult()
    {
        // Arrange
        var command = CreateValidCommand();

        _identityServiceMock
            .Setup(s => s.CreateUserAsync(command.Email, command.Password, command.FirstName, command.LastName, command.Role, It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result<Guid>.Failure("An account with this email already exists."));

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Succeeded.Should().BeFalse();
        result.Errors.Should().Contain("An account with this email already exists.");
    }

    [Fact]
    public async Task Handle_WhenUserCreationFails_DoesNotSendVerificationEmail()
    {
        // Arrange
        var command = CreateValidCommand();

        _identityServiceMock
            .Setup(s => s.CreateUserAsync(command.Email, command.Password, command.FirstName, command.LastName, command.Role, It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result<Guid>.Failure("Something went wrong."));

        // Act
        await _handler.Handle(command, CancellationToken.None);

        // Assert
        _emailServiceMock.Verify(
            e => e.SendEmailVerificationAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CancellationToken>()),
            Times.Never);
    }

    [Fact]
    public async Task Handle_WhenUserCreationSucceeds_PublishesUserRegisteredEvent()
    {
        // Arrange
        var command = CreateValidCommand();
        var newUserId = Guid.NewGuid();

        _identityServiceMock
            .Setup(s => s.CreateUserAsync(command.Email, command.Password, command.FirstName, command.LastName, command.Role, It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result<Guid>.Success(newUserId));

        _identityServiceMock
            .Setup(s => s.GenerateNewEmailVerificationCodeAsync(newUserId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result<string>.Success("123456"));

        // Act
        await _handler.Handle(command, CancellationToken.None);

        // Assert
        _publisherMock.Verify(p => p.Publish(It.IsAny<object>(), It.IsAny<CancellationToken>()), Times.Once);
    }
}