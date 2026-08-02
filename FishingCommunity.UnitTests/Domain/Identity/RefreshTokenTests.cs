using FishingCommunity.Domain.Entities.Identity;
using FluentAssertions;
using Xunit;

namespace FishingCommunity.UnitTests.Domain.Identity;

public class RefreshTokenTests
{
    [Fact]
    public void Constructor_CreatesTokenWithCorrectInitialState()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var expiresOn = DateTime.UtcNow.AddDays(7);

        // Act
        var token = new RefreshToken(userId, "sample-token-value", expiresOn, "127.0.0.1");

        // Assert
        token.UserId.Should().Be(userId);
        token.Token.Should().Be("sample-token-value");
        token.IsRevoked.Should().BeFalse();
        token.IsExpired.Should().BeFalse();
        token.IsActive.Should().BeTrue();
    }

    [Fact]
    public void IsExpired_WhenExpiryDateInPast_ReturnsTrue()
    {
        // Arrange
        var token = new RefreshToken(Guid.NewGuid(), "token", DateTime.UtcNow.AddSeconds(-1), "127.0.0.1");

        // Assert
        token.IsExpired.Should().BeTrue();
        token.IsActive.Should().BeFalse();
    }

    [Fact]
    public void Revoke_SetsRevokedPropertiesCorrectly()
    {
        // Arrange
        var token = new RefreshToken(Guid.NewGuid(), "token", DateTime.UtcNow.AddDays(7), "127.0.0.1");

        // Act
        token.Revoke("192.168.1.1", "new-replacement-token");

        // Assert
        token.IsRevoked.Should().BeTrue();
        token.IsActive.Should().BeFalse();
        token.RevokedByIp.Should().Be("192.168.1.1");
        token.ReplacedByToken.Should().Be("new-replacement-token");
    }

    [Fact]
    public void IsActive_WhenNeitherExpiredNorRevoked_ReturnsTrue()
    {
        // Arrange
        var token = new RefreshToken(Guid.NewGuid(), "token", DateTime.UtcNow.AddDays(1), "127.0.0.1");

        // Assert
        token.IsActive.Should().BeTrue();
    }

    [Fact]
    public void IsActive_WhenRevokedButNotExpired_ReturnsFalse()
    {
        // Arrange
        var token = new RefreshToken(Guid.NewGuid(), "token", DateTime.UtcNow.AddDays(1), "127.0.0.1");
        token.Revoke("127.0.0.1");

        // Assert
        token.IsActive.Should().BeFalse();
    }
}