using FishingCommunity.Domain.Common;

namespace FishingCommunity.Domain.Events.Trips;

public class TripCreatedEvent : DomainEvent
{
    public Guid TripId { get; }
    public Guid OrganizerId { get; }

    public TripCreatedEvent(Guid tripId, Guid organizerId)
    {
        TripId = tripId;
        OrganizerId = organizerId;
    }
}