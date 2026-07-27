using FishingCommunity.Domain.Common;

namespace FishingCommunity.Domain.Events.Trips;

public class TripBookingCancelledEvent : DomainEvent
{
    public Guid TripId { get; }
    public Guid BookingId { get; }
    public Guid UserId { get; }

    public TripBookingCancelledEvent(Guid tripId, Guid bookingId, Guid userId)
    {
        TripId = tripId;
        BookingId = bookingId;
        UserId = userId;
    }
}