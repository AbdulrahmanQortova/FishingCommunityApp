using FishingCommunity.Domain.Common;

namespace FishingCommunity.Domain.Entities.Trips;

public class TripWaitingListEntry : BaseAuditableEntity
{
    public Guid TripId { get; private set; }
    public Trip Trip { get; private set; } = null!;

    public Guid UserId { get; private set; }
    public int SeatsRequested { get; private set; }
    public bool IsPromoted { get; private set; }
    public DateTime? PromotedDate { get; private set; }

    private TripWaitingListEntry() { } // EF Core

    internal TripWaitingListEntry(Guid tripId, Guid userId, int seatsRequested)
    {
        TripId = tripId;
        UserId = userId;
        SeatsRequested = seatsRequested;
    }

    internal void Promote()
    {
        IsPromoted = true;
        PromotedDate = DateTime.UtcNow;
    }
}