namespace FishingCommunity.Application.Features.Notifications.IntegrationEvents;

public class TripBookingRejectedIntegrationEvent
{
    public Guid TripId { get; set; }
    public Guid UserId { get; set; }
}