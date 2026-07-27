using FishingCommunity.Domain.Common;

namespace FishingCommunity.Domain.Events.Trips;

public class TripCancelledEvent : DomainEvent
{
    public Guid TripId { get; }
    public string? Reason { get; }

    public TripCancelledEvent(Guid tripId, string? reason)
    {
        TripId = tripId;
        Reason = reason;
    }
}