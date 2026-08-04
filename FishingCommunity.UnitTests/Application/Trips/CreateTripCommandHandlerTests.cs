using FishingCommunity.Application.Features.Trips.Commands.CreateTrip;
using FishingCommunity.Domain.Entities.Shop;
using FishingCommunity.Domain.Entities.Trips;
using FishingCommunity.Domain.Enums;
using FishingCommunity.Domain.Interfaces;
using FluentAssertions;
using Moq;
using Xunit;

namespace FishingCommunity.UnitTests.Application.Trips;

public class CreateTripCommandHandlerTests
{
    private readonly Mock<IUnitOfWork> _unitOfWorkMock = new();
    private readonly Mock<IRepository<Boat>> _boatRepoMock = new();
    private readonly Mock<IRepository<Trip>> _tripRepoMock = new();
    private readonly CreateTripCommandHandler _handler;

    public CreateTripCommandHandlerTests()
    {
        _unitOfWorkMock.Setup(u => u.Repository<Boat>()).Returns(_boatRepoMock.Object);
        _unitOfWorkMock.Setup(u => u.Repository<Trip>()).Returns(_tripRepoMock.Object);

        _handler = new CreateTripCommandHandler(_unitOfWorkMock.Object);
    }

    private static Boat CreateActiveBoat(Guid ownerId, int capacity = 6)
    {
        return new Boat(ownerId, "Sea Explorer", "REG-001", capacity);
    }

    private static CreateTripCommand CreateValidCommand(Guid organizerId, Guid boatId) => new()
    {
        OrganizerId = organizerId,
        BoatId = boatId,
        Title = "Morning Trip",
        LocationName = "Hurghada",
        Latitude = 27.25,
        Longitude = 33.81,
        DepartureDateTime = DateTime.UtcNow.AddDays(2),
        Capacity = 4,
        PricePerPerson = 200m
    };

    [Fact]
    public async Task Handle_WhenBoatDoesNotExist_ThrowsNotFoundException()
    {
        // Arrange
        var organizerId = Guid.NewGuid();
        var command = CreateValidCommand(organizerId, Guid.NewGuid());

        _boatRepoMock.Setup(r => r.GetByIdAsync(command.BoatId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Boat?)null);

        // Act
        var act = () => _handler.Handle(command, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<FishingCommunity.Domain.Exceptions.NotFoundException>();
    }

    [Fact]
    public async Task Handle_WhenRequestingUserIsNotBoatOwner_ReturnsFailure()
    {
        // Arrange
        var actualOwnerId = Guid.NewGuid();
        var impersonatorId = Guid.NewGuid();
        var boat = CreateActiveBoat(actualOwnerId);
        var command = CreateValidCommand(impersonatorId, boat.Id);

        _boatRepoMock.Setup(r => r.GetByIdAsync(boat.Id, It.IsAny<CancellationToken>())).ReturnsAsync(boat);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Succeeded.Should().BeFalse();
        result.Errors.Should().Contain("You can only create trips for boats you own.");
    }

    [Fact]
    public async Task Handle_WhenBoatUnderMaintenance_ReturnsFailure()
    {
        // Arrange
        var ownerId = Guid.NewGuid();
        var boat = CreateActiveBoat(ownerId);
        boat.MarkUnderMaintenance();
        var command = CreateValidCommand(ownerId, boat.Id);

        _boatRepoMock.Setup(r => r.GetByIdAsync(boat.Id, It.IsAny<CancellationToken>())).ReturnsAsync(boat);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Succeeded.Should().BeFalse();
        result.Errors.Should().Contain(e => e.Contains("not currently active"));
    }

    [Fact]
    public async Task Handle_WhenTripCapacityExceedsBoatCapacity_ReturnsFailure()
    {
        // Arrange
        var ownerId = Guid.NewGuid();
        var boat = CreateActiveBoat(ownerId, capacity: 4);
        var command = CreateValidCommand(ownerId, boat.Id);
        command.Capacity = 10; // Exceeds boat's capacity of 4.

        _boatRepoMock.Setup(r => r.GetByIdAsync(boat.Id, It.IsAny<CancellationToken>())).ReturnsAsync(boat);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Succeeded.Should().BeFalse();
        result.Errors.Should().Contain(e => e.Contains("cannot exceed the boat's capacity"));
    }

    [Fact]
    public async Task Handle_WithValidData_CreatesTripAndSavesChanges()
    {
        // Arrange
        var ownerId = Guid.NewGuid();
        var boat = CreateActiveBoat(ownerId, capacity: 6);
        var command = CreateValidCommand(ownerId, boat.Id);

        _boatRepoMock.Setup(r => r.GetByIdAsync(boat.Id, It.IsAny<CancellationToken>())).ReturnsAsync(boat);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Succeeded.Should().BeTrue();
        result.Data!.Title.Should().Be(command.Title);
        result.Data.Capacity.Should().Be(command.Capacity);

        _tripRepoMock.Verify(r => r.AddAsync(It.IsAny<Trip>(), It.IsAny<CancellationToken>()), Times.Once);
        _unitOfWorkMock.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }
}