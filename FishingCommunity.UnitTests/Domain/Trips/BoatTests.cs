using FishingCommunity.Domain.Entities.Trips;
using FishingCommunity.Domain.Enums;
using FishingCommunity.Domain.Exceptions;
using FluentAssertions;
using Xunit;

namespace FishingCommunity.UnitTests.Domain.Trips;

public class BoatTests
{
    [Fact]
    public void Constructor_WithValidData_CreatesBoatSuccessfully()
    {
        // Act
        var boat = new Boat(Guid.NewGuid(), "Sea Explorer", "REG-001", capacity: 6);

        // Assert
        boat.Name.Should().Be("Sea Explorer");
        boat.Capacity.Should().Be(6);
        boat.Status.Should().Be(BoatStatus.Active);
    }

    [Fact]
    public void Constructor_WithZeroCapacity_ThrowsBusinessRuleValidationException()
    {
        // Act
        var act = () => new Boat(Guid.NewGuid(), "Sea Explorer", "REG-001", capacity: 0);

        // Assert
        act.Should().Throw<BusinessRuleValidationException>()
            .WithMessage("*capacity must be greater than zero*");
    }

    [Fact]
    public void UpdateDetails_WithValidData_UpdatesSuccessfully()
    {
        // Arrange
        var boat = new Boat(Guid.NewGuid(), "Old Name", "REG-001", 6);

        // Act
        boat.UpdateDetails("New Name", "New description", 8);

        // Assert
        boat.Name.Should().Be("New Name");
        boat.Capacity.Should().Be(8);
    }

    [Fact]
    public void UpdateDetails_WithZeroCapacity_ThrowsBusinessRuleValidationException()
    {
        // Arrange
        var boat = new Boat(Guid.NewGuid(), "Boat", "REG-001", 6);

        // Act
        var act = () => boat.UpdateDetails("Boat", null, 0);

        // Assert
        act.Should().Throw<BusinessRuleValidationException>();
    }

    [Fact]
    public void MarkUnderMaintenance_ChangesStatusCorrectly()
    {
        // Arrange
        var boat = new Boat(Guid.NewGuid(), "Boat", "REG-001", 6);

        // Act
        boat.MarkUnderMaintenance();

        // Assert
        boat.Status.Should().Be(BoatStatus.UnderMaintenance);
    }

    [Fact]
    public void Activate_AfterMaintenance_RestoresActiveStatus()
    {
        // Arrange
        var boat = new Boat(Guid.NewGuid(), "Boat", "REG-001", 6);
        boat.MarkUnderMaintenance();

        // Act
        boat.Activate();

        // Assert
        boat.Status.Should().Be(BoatStatus.Active);
    }

    [Fact]
    public void AddPhoto_AddsUrlToCollection()
    {
        // Arrange
        var boat = new Boat(Guid.NewGuid(), "Boat", "REG-001", 6);

        // Act
        boat.AddPhoto("https://example.com/photo1.jpg");

        // Assert
        boat.PhotoUrls.Should().ContainSingle().Which.Should().Be("https://example.com/photo1.jpg");
    }

    [Fact]
    public void RemovePhoto_RemovesUrlFromCollection()
    {
        // Arrange
        var boat = new Boat(Guid.NewGuid(), "Boat", "REG-001", 6);
        boat.AddPhoto("https://example.com/photo1.jpg");

        // Act
        boat.RemovePhoto("https://example.com/photo1.jpg");

        // Assert
        boat.PhotoUrls.Should().BeEmpty();
    }
}