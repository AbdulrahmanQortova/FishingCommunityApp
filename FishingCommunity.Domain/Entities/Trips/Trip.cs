using FishingCommunity.Domain.Common;
using FishingCommunity.Domain.Enums;
using FishingCommunity.Domain.Events.Trips;
using FishingCommunity.Domain.Exceptions;

namespace FishingCommunity.Domain.Entities.Trips;

public class Trip : BaseAuditableEntity, IAggregateRoot
{
    public Guid BoatId { get; private set; }
    public Boat Boat { get; private set; } = null!;

    public Guid OrganizerId { get; private set; } // FK to ApplicationUser (BoatOwner)

    public string Title { get; private set; } = string.Empty;
    public string? Description { get; private set; }

    public DateTime DepartureDateTime { get; private set; }
    public DateTime? EstimatedReturnDateTime { get; private set; }

    public string LocationName { get; private set; } = string.Empty;
    public double Latitude { get; private set; }
    public double Longitude { get; private set; }

    public int Capacity { get; private set; }
    public decimal PricePerPerson { get; private set; }

    public TripStatus Status { get; private set; } = TripStatus.Scheduled;

    private readonly List<string> _photoUrls = new();
    public IReadOnlyCollection<string> PhotoUrls => _photoUrls.AsReadOnly();

    private readonly List<TripBooking> _bookings = new();
    public IReadOnlyCollection<TripBooking> Bookings => _bookings.AsReadOnly();

    private readonly List<TripWaitingListEntry> _waitingList = new();
    public IReadOnlyCollection<TripWaitingListEntry> WaitingList => _waitingList.AsReadOnly();

    private readonly List<TripReview> _reviews = new();
    public IReadOnlyCollection<TripReview> Reviews => _reviews.AsReadOnly();

    public int ApprovedBookingsCount => _bookings.Count(b => b.Status is BookingStatus.Approved or BookingStatus.CheckedIn or BookingStatus.Completed);
    public int AvailableSeats => Capacity - ApprovedBookingsCount;
    public bool IsFull => AvailableSeats <= 0;

    public double? AverageRating => _reviews.Count > 0 ? _reviews.Average(r => r.Rating) : null;

    private Trip() { } // EF Core

    public Trip(
        Guid boatId,
        Guid organizerId,
        string title,
        string locationName,
        double latitude,
        double longitude,
        DateTime departureDateTime,
        int capacity,
        decimal pricePerPerson,
        string? description = null,
        DateTime? estimatedReturnDateTime = null)
    {
        if (capacity <= 0)
        {
            throw new BusinessRuleValidationException("Trip capacity must be greater than zero.");
        }

        if (departureDateTime <= DateTime.UtcNow)
        {
            throw new BusinessRuleValidationException("Trip departure date must be in the future.");
        }

        if (pricePerPerson < 0)
        {
            throw new BusinessRuleValidationException("Trip price cannot be negative.");
        }

        BoatId = boatId;
        OrganizerId = organizerId;
        Title = title;
        LocationName = locationName;
        Latitude = latitude;
        Longitude = longitude;
        DepartureDateTime = departureDateTime;
        EstimatedReturnDateTime = estimatedReturnDateTime;
        Capacity = capacity;
        PricePerPerson = pricePerPerson;
        Description = description;

        AddDomainEvent(new TripCreatedEvent(Id, organizerId));
    }

    public void UpdateDetails(
        string title, string? description, DateTime departureDateTime,
        DateTime? estimatedReturnDateTime, decimal pricePerPerson)
    {
        EnsureNotStartedOrCompleted();

        if (departureDateTime <= DateTime.UtcNow)
        {
            throw new BusinessRuleValidationException("Trip departure date must be in the future.");
        }

        Title = title;
        Description = description;
        DepartureDateTime = departureDateTime;
        EstimatedReturnDateTime = estimatedReturnDateTime;
        PricePerPerson = pricePerPerson;
    }

    public void AddPhoto(string url) => _photoUrls.Add(url);
    public void RemovePhoto(string url) => _photoUrls.Remove(url);

    public TripBooking RequestBooking(Guid userId, int seatsRequested)
    {
        EnsureNotStartedOrCompleted();

        if (Status == TripStatus.Cancelled)
        {
            throw new BusinessRuleValidationException("Cannot book a cancelled trip.");
        }

        if (_bookings.Any(b => b.UserId == userId && b.Status is BookingStatus.Pending or BookingStatus.Approved or BookingStatus.CheckedIn))
        {
            throw new BusinessRuleValidationException("You already have an active booking for this trip.");
        }

        var booking = new TripBooking(Id, userId, seatsRequested);
        _bookings.Add(booking);

        AddDomainEvent(new TripBookingRequestedEvent(Id, booking.Id, userId));

        return booking;
    }

    public void ApproveBooking(Guid bookingId)
    {
        var booking = GetBookingOrThrow(bookingId);

        if (AvailableSeats < booking.SeatsRequested)
        {
            throw new BusinessRuleValidationException("Not enough available seats to approve this booking.");
        }

        booking.Approve();

        AddDomainEvent(new TripBookingApprovedEvent(Id, bookingId, booking.UserId));
    }

    public void RejectBooking(Guid bookingId, string? reason)
    {
        var booking = GetBookingOrThrow(bookingId);
        booking.Reject(reason);

        AddDomainEvent(new TripBookingRejectedEvent(Id, bookingId, booking.UserId));
    }

    public void CancelBooking(Guid bookingId, Guid cancelledBy)
    {
        var booking = GetBookingOrThrow(bookingId);
        booking.Cancel();

        AddDomainEvent(new TripBookingCancelledEvent(Id, bookingId, booking.UserId));

        // Promote the next person in the waiting list, if any, now that a seat freed up.
        PromoteNextFromWaitingList();
    }

    public TripWaitingListEntry JoinWaitingList(Guid userId, int seatsRequested)
    {
        if (!IsFull)
        {
            throw new BusinessRuleValidationException("Trip still has available seats — book directly instead of joining the waiting list.");
        }

        if (_waitingList.Any(w => w.UserId == userId && !w.IsPromoted))
        {
            throw new BusinessRuleValidationException("You are already on the waiting list for this trip.");
        }

        var entry = new TripWaitingListEntry(Id, userId, seatsRequested);
        _waitingList.Add(entry);

        return entry;
    }

    private void PromoteNextFromWaitingList()
    {
        var next = _waitingList
            .Where(w => !w.IsPromoted)
            .OrderBy(w => w.CreatedDate)
            .FirstOrDefault(w => w.SeatsRequested <= AvailableSeats);

        if (next is null) return;

        next.Promote();
        RequestBooking(next.UserId, next.SeatsRequested);
    }

    public void Cancel(string? reason)
    {
        EnsureNotStartedOrCompleted();

        Status = TripStatus.Cancelled;

        foreach (var booking in _bookings.Where(b => b.Status is BookingStatus.Pending or BookingStatus.Approved))
        {
            booking.Cancel();
        }

        AddDomainEvent(new TripCancelledEvent(Id, reason));
    }

    public void Start()
    {
        if (Status != TripStatus.Scheduled)
        {
            throw new BusinessRuleValidationException("Only a scheduled trip can be started.");
        }

        Status = TripStatus.InProgress;
    }

    public void Complete()
    {
        if (Status != TripStatus.InProgress)
        {
            throw new BusinessRuleValidationException("Only a trip in progress can be marked as completed.");
        }

        Status = TripStatus.Completed;

        foreach (var booking in _bookings.Where(b => b.Status == BookingStatus.CheckedIn))
        {
            booking.Complete();
        }

        AddDomainEvent(new TripCompletedEvent(Id));
    }

    public TripReview AddReview(Guid userId, int rating, string? comment)
    {
        if (Status != TripStatus.Completed)
        {
            throw new BusinessRuleValidationException("You can only review a completed trip.");
        }

        var hasCompletedBooking = _bookings.Any(b => b.UserId == userId && b.Status == BookingStatus.Completed);
        if (!hasCompletedBooking)
        {
            throw new BusinessRuleValidationException("Only participants who completed this trip can leave a review.");
        }

        if (_reviews.Any(r => r.UserId == userId))
        {
            throw new BusinessRuleValidationException("You have already reviewed this trip.");
        }

        var review = new TripReview(Id, userId, rating, comment);
        _reviews.Add(review);

        return review;
    }

    private TripBooking GetBookingOrThrow(Guid bookingId)
    {
        var booking = _bookings.FirstOrDefault(b => b.Id == bookingId);

        if (booking is null)
        {
            throw new NotFoundException(nameof(TripBooking), bookingId);
        }

        return booking;
    }
    public void CheckInBooking(Guid bookingId)
    {
        var booking = GetBookingOrThrow(bookingId);
        booking.CheckIn();
    }
    private void EnsureNotStartedOrCompleted()
    {
        if (Status is TripStatus.InProgress or TripStatus.Completed or TripStatus.Cancelled)
        {
            throw new BusinessRuleValidationException("This action is not allowed once the trip has started, completed, or been cancelled.");
        }
    }
}