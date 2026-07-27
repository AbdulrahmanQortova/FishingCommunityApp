using FishingCommunity.Domain.Common;
using FishingCommunity.Domain.Enums;
using FishingCommunity.Domain.Exceptions;

namespace FishingCommunity.Domain.Entities.Trips;

public class TripBooking : BaseAuditableEntity
{
    public Guid TripId { get; private set; }
    public Trip Trip { get; private set; } = null!;

    public Guid UserId { get; private set; } // FK to ApplicationUser

    public int SeatsRequested { get; private set; }
    public BookingStatus Status { get; private set; }

    public DateTime? ApprovedDate { get; private set; }
    public DateTime? RejectedDate { get; private set; }
    public string? RejectionReason { get; private set; }
    public DateTime? CancelledDate { get; private set; }
    public DateTime? CheckedInDate { get; private set; }

    private TripBooking() { } // EF Core

    internal TripBooking(Guid tripId, Guid userId, int seatsRequested)
    {
        if (seatsRequested <= 0)
        {
            throw new BusinessRuleValidationException("Seats requested must be greater than zero.");
        }

        TripId = tripId;
        UserId = userId;
        SeatsRequested = seatsRequested;
        Status = BookingStatus.Pending;
    }

    internal void Approve()
    {
        if (Status != BookingStatus.Pending)
        {
            throw new BusinessRuleValidationException("Only a pending booking can be approved.");
        }

        Status = BookingStatus.Approved;
        ApprovedDate = DateTime.UtcNow;
    }

    internal void Reject(string? reason)
    {
        if (Status != BookingStatus.Pending)
        {
            throw new BusinessRuleValidationException("Only a pending booking can be rejected.");
        }

        Status = BookingStatus.Rejected;
        RejectedDate = DateTime.UtcNow;
        RejectionReason = reason;
    }

    internal void Cancel()
    {
        if (Status is BookingStatus.Completed or BookingStatus.Cancelled)
        {
            throw new BusinessRuleValidationException("This booking cannot be cancelled.");
        }

        Status = BookingStatus.Cancelled;
        CancelledDate = DateTime.UtcNow;
    }

    internal void CheckIn()
    {
        if (Status != BookingStatus.Approved)
        {
            throw new BusinessRuleValidationException("Only an approved booking can be checked in.");
        }

        Status = BookingStatus.CheckedIn;
        CheckedInDate = DateTime.UtcNow;
    }

    internal void MarkAsNoShow()
    {
        if (Status != BookingStatus.Approved)
        {
            throw new BusinessRuleValidationException("Only an approved booking can be marked as no-show.");
        }

        Status = BookingStatus.NoShow;
    }

    internal void Complete()
    {
        if (Status != BookingStatus.CheckedIn)
        {
            throw new BusinessRuleValidationException("Only a checked-in booking can be marked as completed.");
        }

        Status = BookingStatus.Completed;
    }
}