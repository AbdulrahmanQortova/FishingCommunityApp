using FishingCommunity.Domain.Common;

namespace FishingCommunity.Domain.Events.Trips;

public class TripCompletedEvent : DomainEvent
{
    public Guid TripId { get; }

    public TripCompletedEvent(Guid tripId)
    {
        TripId = tripId;
    }
}