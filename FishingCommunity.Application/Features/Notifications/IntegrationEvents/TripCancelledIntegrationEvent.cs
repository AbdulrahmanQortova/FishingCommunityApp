namespace FishingCommunity.Application.Features.Notifications.IntegrationEvents;

public class TripCancelledIntegrationEvent
{
    public Guid TripId { get; set; }
    public string? Reason { get; set; }
    public List<Guid> AffectedUserIds { get; set; } = new();
}