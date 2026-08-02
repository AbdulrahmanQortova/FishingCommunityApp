using FishingCommunity.Application.Common.Interfaces;
using FishingCommunity.Application.Common.Models;
using FishingCommunity.Application.Features.Auth.Commands.Login;
using FishingCommunity.Domain.Entities.Identity;
using FishingCommunity.Domain.Interfaces;
using FishingCommunity.Shared.Wrappers;
using FluentAssertions;
using Microsoft.Extensions.Options;
using Moq;
using Xunit;

namespace FishingCommunity.UnitTests.Application.Auth;

public class LoginCommandHandlerTests
{
    private readonly Mock<IIdentityService> _identityServiceMock = new();
    private readonly Mock<IJwtTokenService> _jwtTokenServiceMock = new();
    private readonly Mock<IUnitOfWork> _unitOfWorkMock = new();
    private readonly Mock<IRepository<RefreshToken>> _refreshTokenRepoMock = new();
    private readonly Mock<IDateTimeProvider> _dateTimeProviderMock = new();
    private readonly LoginCommandHandler _handler;

    private readonly DateTime _fixedNow = new(2026, 1, 1, 12, 0, 0, DateTimeKind.Utc);

    public LoginCommandHandlerTests()
    {
        _unitOfWorkMock.Setup(u => u.Repository<RefreshToken>()).Returns(_refreshTokenRepoMock.Object);
        _dateTimeProviderMock.Setup(d => d.UtcNow).Returns(_fixedNow);

        _handler = new LoginCommandHandler(
            _identityServiceMock.Object,
            _jwtTokenServiceMock.Object,
            _unitOfWorkMock.Object,
            _dateTimeProviderMock.Object,
            Options.Create(new FeatureFlags { RequireEmailVerification = true }));
    }

    private static LoginCommand CreateCommand() => new()
    {
        Email = "user@example.com",
        Password = "Pass@word1",
        IpAddress = "127.0.0.1"
    };

    [Fact]
    public async Task Handle_WithInvalidCredentials_ReturnsFailure()
    {
        // Arrange
        var command = CreateCommand();

        _identityServiceMock
            .Setup(s => s.ValidateCredentialsAsync(command.Email, command.Password, It.IsAny<CancellationToken>()))
            .ReturnsAsync((false, (Guid?)null, new List<string>()));

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Succeeded.Should().BeFalse();
        result.Errors.Should().Contain("Invalid email or password.");
    }

    [Fact]
    public async Task Handle_WhenEmailNotVerified_ReturnsFailure()
    {
        // Arrange
        var command = CreateCommand();
        var userId = Guid.NewGuid();

        _identityServiceMock
            .Setup(s => s.ValidateCredentialsAsync(command.Email, command.Password, It.IsAny<CancellationToken>()))
            .ReturnsAsync((true, userId, new List<string> { "RegularUser" }));

        _identityServiceMock
            .Setup(s => s.GetUserProfileAsync(userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new UserProfileDto { UserId = userId, Email = command.Email, IsEmailVerified = false });

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Succeeded.Should().BeFalse();
        result.Errors.Should().Contain("Please verify your email before logging in.");
    }

    [Fact]
    public async Task Handle_WithValidCredentialsAndVerifiedEmail_ReturnsAccessAndRefreshTokens()
    {
        // Arrange
        var command = CreateCommand();
        var userId = Guid.NewGuid();
        var roles = new List<string> { "RegularUser" };

        _identityServiceMock
            .Setup(s => s.ValidateCredentialsAsync(command.Email, command.Password, It.IsAny<CancellationToken>()))
            .ReturnsAsync((true, userId, roles));

        _identityServiceMock
            .Setup(s => s.GetUserProfileAsync(userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new UserProfileDto { UserId = userId, Email = command.Email, FirstName = "Ahmed", LastName = "Test", IsEmailVerified = true });

        _jwtTokenServiceMock
            .Setup(j => j.GenerateAccessToken(userId, command.Email, roles))
            .Returns("fake-access-token");

        _jwtTokenServiceMock
            .Setup(j => j.GenerateRefreshToken())
            .Returns("fake-refresh-token");

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Succeeded.Should().BeTrue();
        result.Data!.AccessToken.Should().Be("fake-access-token");
        result.Data.RefreshToken.Should().Be("fake-refresh-token");

        _refreshTokenRepoMock.Verify(r => r.AddAsync(It.IsAny<RefreshToken>(), It.IsAny<CancellationToken>()), Times.Once);
        _unitOfWorkMock.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_WhenFeatureFlagDisablesEmailVerification_LogsInDespiteUnverifiedEmail()
    {
        // Arrange: create a handler with RequireEmailVerification = false.
        var handlerWithFlagOff = new LoginCommandHandler(
            _identityServiceMock.Object,
            _jwtTokenServiceMock.Object,
            _unitOfWorkMock.Object,
            _dateTimeProviderMock.Object,
            Options.Create(new FeatureFlags { RequireEmailVerification = false }));

        var command = CreateCommand();
        var userId = Guid.NewGuid();

        _identityServiceMock
            .Setup(s => s.ValidateCredentialsAsync(command.Email, command.Password, It.IsAny<CancellationToken>()))
            .ReturnsAsync((true, userId, new List<string> { "RegularUser" }));

        _identityServiceMock
            .Setup(s => s.GetUserProfileAsync(userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new UserProfileDto { UserId = userId, Email = command.Email, IsEmailVerified = false }); // Not verified!

        _jwtTokenServiceMock.Setup(j => j.GenerateAccessToken(userId, command.Email, It.IsAny<IList<string>>())).Returns("token");
        _jwtTokenServiceMock.Setup(j => j.GenerateRefreshToken()).Returns("refresh");

        // Act
        var result = await handlerWithFlagOff.Handle(command, CancellationToken.None);

        // Assert
        result.Succeeded.Should().BeTrue(); // Login succeeds despite unverified email, since the flag is off.
    }
}