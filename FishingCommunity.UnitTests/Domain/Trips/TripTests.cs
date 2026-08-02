using FishingCommunity.Domain.Entities.Trips;
using FishingCommunity.Domain.Enums;
using FishingCommunity.Domain.Exceptions;
using FluentAssertions;
using Xunit;

namespace FishingCommunity.UnitTests.Domain.Trips;

public class TripTests
{
    private static Trip CreateValidTrip(int capacity = 4, decimal price = 100m)
    {
        return new Trip(
            boatId: Guid.NewGuid(),
            organizerId: Guid.NewGuid(),
            title: "Morning Fishing Trip",
            locationName: "Hurghada",
            latitude: 27.25,
            longitude: 33.81,
            departureDateTime: DateTime.UtcNow.AddDays(1),
            capacity: capacity,
            pricePerPerson: price);
    }

    [Fact]
    public void Constructor_WithValidData_CreatesTripSuccessfully()
    {
        // Act
        var trip = CreateValidTrip();

        // Assert
        trip.Should().NotBeNull();
        trip.Status.Should().Be(TripStatus.Scheduled);
        trip.AvailableSeats.Should().Be(4);
        trip.IsFull.Should().BeFalse();
    }

    [Fact]
    public void Constructor_WithZeroCapacity_ThrowsBusinessRuleValidationException()
    {
        // Act
        var act = () => new Trip(
            Guid.NewGuid(), Guid.NewGuid(), "Trip", "Location", 0, 0,
            DateTime.UtcNow.AddDays(1), capacity: 0, pricePerPerson: 100m);

        // Assert
        act.Should().Throw<BusinessRuleValidationException>()
            .WithMessage("*capacity must be greater than zero*");
    }

    [Fact]
    public void Constructor_WithPastDepartureDate_ThrowsBusinessRuleValidationException()
    {
        // Act
        var act = () => new Trip(
            Guid.NewGuid(), Guid.NewGuid(), "Trip", "Location", 0, 0,
            departureDateTime: DateTime.UtcNow.AddDays(-1), capacity: 4, pricePerPerson: 100m);

        // Assert
        act.Should().Throw<BusinessRuleValidationException>()
            .WithMessage("*must be in the future*");
    }

    [Fact]
    public void Constructor_WithNegativePrice_ThrowsBusinessRuleValidationException()
    {
        // Act
        var act = () => new Trip(
            Guid.NewGuid(), Guid.NewGuid(), "Trip", "Location", 0, 0,
            DateTime.UtcNow.AddDays(1), capacity: 4, pricePerPerson: -50m);

        // Assert
        act.Should().Throw<BusinessRuleValidationException>()
            .WithMessage("*cannot be negative*");
    }

    [Fact]
    public void RequestBooking_WhenSeatsAvailable_CreatesPendingBooking()
    {
        // Arrange
        var trip = CreateValidTrip(capacity: 4);
        var userId = Guid.NewGuid();

        // Act
        var booking = trip.RequestBooking(userId, seatsRequested: 2);

        // Assert
        booking.Status.Should().Be(BookingStatus.Pending);
        booking.SeatsRequested.Should().Be(2);
        trip.Bookings.Should().ContainSingle();
    }

    [Fact]
    public void RequestBooking_WhenUserAlreadyHasActiveBooking_ThrowsBusinessRuleValidationException()
    {
        // Arrange
        var trip = CreateValidTrip(capacity: 4);
        var userId = Guid.NewGuid();
        trip.RequestBooking(userId, 1);

        // Act
        var act = () => trip.RequestBooking(userId, 1);

        // Assert
        act.Should().Throw<BusinessRuleValidationException>()
            .WithMessage("*already have an active booking*");
    }

    [Fact]
    public void ApproveBooking_WhenEnoughSeatsAvailable_ApprovesSuccessfully()
    {
        // Arrange
        var trip = CreateValidTrip(capacity: 4);
        var booking = trip.RequestBooking(Guid.NewGuid(), 2);

        // Act
        trip.ApproveBooking(booking.Id);

        // Assert
        booking.Status.Should().Be(BookingStatus.Approved);
        trip.AvailableSeats.Should().Be(2);
    }

    [Fact]
    public void ApproveBooking_WhenNotEnoughSeatsAvailable_ThrowsBusinessRuleValidationException()
    {
        // Arrange
        var trip = CreateValidTrip(capacity: 2);
        var booking1 = trip.RequestBooking(Guid.NewGuid(), 2);
        trip.ApproveBooking(booking1.Id); // Fills the trip.

        var booking2 = trip.RequestBooking(Guid.NewGuid(), 1); // Different user, still pending.

        // Act
        var act = () => trip.ApproveBooking(booking2.Id);

        // Assert
        act.Should().Throw<BusinessRuleValidationException>()
            .WithMessage("*Not enough available seats*");
    }

    [Fact]
    public void CancelBooking_WhenWaitingListHasEligibleUser_PromotesNextInLine()
    {
        // Arrange: trip with 1 seat, one approved booking fills it, another user waits.
        var trip = CreateValidTrip(capacity: 1);

        var firstUserBooking = trip.RequestBooking(Guid.NewGuid(), 1);
        trip.ApproveBooking(firstUserBooking.Id); // Trip is now full.

        var waitingUserId = Guid.NewGuid();
        trip.JoinWaitingList(waitingUserId, 1);

        // Act: the first user cancels, freeing up a seat.
        trip.CancelBooking(firstUserBooking.Id, firstUserBooking.UserId);

        // Assert: the waiting user should now have an active (pending) booking.
        trip.Bookings.Should().Contain(b => b.UserId == waitingUserId && b.Status == BookingStatus.Pending);
        trip.WaitingList.Should().Contain(w => w.UserId == waitingUserId && w.IsPromoted);
    }

    [Fact]
    public void JoinWaitingList_WhenTripHasAvailableSeats_ThrowsBusinessRuleValidationException()
    {
        // Arrange
        var trip = CreateValidTrip(capacity: 4); // Not full.

        // Act
        var act = () => trip.JoinWaitingList(Guid.NewGuid(), 1);

        // Assert
        act.Should().Throw<BusinessRuleValidationException>()
            .WithMessage("*still has available seats*");
    }

    [Fact]
    public void Cancel_WhenTripHasActiveBookings_CancelsAllBookingsToo()
    {
        // Arrange
        var trip = CreateValidTrip(capacity: 4);
        var booking = trip.RequestBooking(Guid.NewGuid(), 1);
        trip.ApproveBooking(booking.Id);

        // Act
        trip.Cancel("Weather conditions unsafe");

        // Assert
        trip.Status.Should().Be(TripStatus.Cancelled);
        booking.Status.Should().Be(BookingStatus.Cancelled);
    }

    [Fact]
    public void Cancel_WhenTripAlreadyInProgress_ThrowsBusinessRuleValidationException()
    {
        // Arrange
        var trip = CreateValidTrip();
        trip.Start();

        // Act
        var act = () => trip.Cancel(null);

        // Assert
        act.Should().Throw<BusinessRuleValidationException>();
    }

    [Fact]
    public void Complete_WhenTripInProgress_MarksCheckedInBookingsAsCompleted()
    {
        // Arrange
        var trip = CreateValidTrip();
        var booking = trip.RequestBooking(Guid.NewGuid(), 1);
        trip.ApproveBooking(booking.Id);
        trip.Start();
        trip.CheckInBooking(booking.Id);

        // Act
        trip.Complete();

        // Assert
        trip.Status.Should().Be(TripStatus.Completed);
        booking.Status.Should().Be(BookingStatus.Completed);
    }

    [Fact]
    public void Complete_WhenTripNotInProgress_ThrowsBusinessRuleValidationException()
    {
        // Arrange
        var trip = CreateValidTrip(); // Still Scheduled, never started.

        // Act
        var act = () => trip.Complete();

        // Assert
        act.Should().Throw<BusinessRuleValidationException>()
            .WithMessage("*trip in progress can be marked*");
    }

    [Fact]
    public void AddReview_WhenTripCompletedAndUserParticipated_AddsReviewSuccessfully()
    {
        // Arrange
        var trip = CreateValidTrip();
        var userId = Guid.NewGuid();
        var booking = trip.RequestBooking(userId, 1);
        trip.ApproveBooking(booking.Id);
        trip.Start();
        trip.CheckInBooking(booking.Id);
        trip.Complete();

        // Act
        var review = trip.AddReview(userId, rating: 5, comment: "Amazing trip!");

        // Assert
        review.Rating.Should().Be(5);
        trip.AverageRating.Should().Be(5);
    }

    [Fact]
    public void AddReview_WhenUserDidNotParticipate_ThrowsBusinessRuleValidationException()
    {
        // Arrange
        var trip = CreateValidTrip();
        var participantId = Guid.NewGuid();
        var booking = trip.RequestBooking(participantId, 1);
        trip.ApproveBooking(booking.Id);
        trip.Start();
        trip.CheckInBooking(booking.Id);
        trip.Complete();

        var strangerId = Guid.NewGuid();

        // Act
        var act = () => trip.AddReview(strangerId, 5, "Great!");

        // Assert
        act.Should().Throw<BusinessRuleValidationException>()
            .WithMessage("*Only participants*");
    }

    [Theory]
    [InlineData(0)]
    [InlineData(6)]
    public void AddReview_WithRatingOutOfRange_ThrowsBusinessRuleValidationException(int invalidRating)
    {
        // Arrange
        var trip = CreateValidTrip();
        var userId = Guid.NewGuid();
        var booking = trip.RequestBooking(userId, 1);
        trip.ApproveBooking(booking.Id);
        trip.Start();
        trip.CheckInBooking(booking.Id);
        trip.Complete();

        // Act
        var act = () => trip.AddReview(userId, invalidRating, null);

        // Assert
        act.Should().Throw<BusinessRuleValidationException>()
            .WithMessage("*Rating must be between 1 and 5*");
    }
}