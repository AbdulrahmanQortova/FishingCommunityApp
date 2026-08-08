namespace FishingCommunity.Application.Features.Notifications.IntegrationEvents;

public class TripBookingApprovedIntegrationEvent
{
    public Guid TripId { get; set; }
    public Guid UserId { get; set; }
}