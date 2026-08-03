using FishingCommunity.Application.Features.Trips.Bookings.Commands.RequestBooking;
using FishingCommunity.Domain.Entities.Trips;
using FishingCommunity.Infrastructure.Persistence;
using FishingCommunity.Infrastructure.Persistence.Repositories;
using FishingCommunity.UnitTests.Common;
using FluentAssertions;
using Moq;
using Xunit;
using FishingCommunity.Domain.Interfaces;

namespace FishingCommunity.UnitTests.Application.Trips;

public class RequestBookingCommandHandlerTests : IDisposable
{
    private readonly ApplicationDbContext _dbContext;
    private readonly Mock<IUnitOfWork> _unitOfWorkMock = new();
    private readonly RequestBookingCommandHandler _handler;

    public RequestBookingCommandHandlerTests()
    {
        _dbContext = InMemoryDbContextFactory.Create();

        // Wire the real Repository implementation against the in-memory context —
        // this makes .Include()/.FirstOrDefaultAsync() behave exactly as they would
        // against SQL Server, since it's genuinely going through EF Core's query pipeline.
        var tripRepository = new Repository<Trip>(_dbContext);
        _unitOfWorkMock.Setup(u => u.Repository<Trip>()).Returns(tripRepository);
        _unitOfWorkMock.Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .Returns(() => _dbContext.SaveChangesAsync());

        _handler = new RequestBookingCommandHandler(_unitOfWorkMock.Object);
    }

    private async Task<Trip> SeedTripAsync(int capacity)
    {
        var trip = new Trip(
            Guid.NewGuid(), Guid.NewGuid(), "Trip", "Location", 0, 0,
            DateTime.UtcNow.AddDays(1), capacity, 100m);

        _dbContext.Trips.Add(trip);
        await _dbContext.SaveChangesAsync();

        return trip;
    }

    [Fact]
    public async Task Handle_WhenTripHasAvailableSeats_CreatesApprovedPendingBooking()
    {
        // Arrange
        var trip = await SeedTripAsync(capacity: 4);
        var command = new RequestBookingCommand { TripId = trip.Id, UserId = Guid.NewGuid(), SeatsRequested = 2 };

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Succeeded.Should().BeTrue();
        result.Data!.WasAddedToWaitingList.Should().BeFalse();

        var reloadedTrip = await _dbContext.Trips.FindAsync(trip.Id);
        reloadedTrip!.Bookings.Should().ContainSingle(b => b.UserId == command.UserId);
    }

    [Fact]
    public async Task Handle_WhenTripIsFull_AddsUserToWaitingListInstead()
    {
        // Arrange
        var trip = await SeedTripAsync(capacity: 1);
        var firstUserId = Guid.NewGuid();

        var existingBooking = trip.RequestBooking(firstUserId, 1);
        trip.ApproveBooking(existingBooking.Id);
        await _dbContext.SaveChangesAsync();

        var newUserId = Guid.NewGuid();
        var command = new RequestBookingCommand { TripId = trip.Id, UserId = newUserId, SeatsRequested = 1 };

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Succeeded.Should().BeTrue();
        result.Data!.WasAddedToWaitingList.Should().BeTrue();
    }

    [Fact]
    public async Task Handle_WhenTripDoesNotExist_ThrowsNotFoundException()
    {
        // Arrange
        var command = new RequestBookingCommand { TripId = Guid.NewGuid(), UserId = Guid.NewGuid(), SeatsRequested = 1 };

        // Act
        var act = () => _handler.Handle(command, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<FishingCommunity.Domain.Exceptions.NotFoundException>();
    }

    public void Dispose()
    {
        _dbContext.Dispose();
    }
}