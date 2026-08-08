namespace FishingCommunity.Application.Features.Notifications.IntegrationEvents;

public class TripBookingRequestedIntegrationEvent
{
    public Guid TripId { get; set; }
    public string TripTitle { get; set; } = string.Empty;
    public Guid OrganizerId { get; set; }
}