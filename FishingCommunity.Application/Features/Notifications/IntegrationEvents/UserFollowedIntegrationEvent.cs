namespace FishingCommunity.Application.Features.Notifications.IntegrationEvents;

public class UserFollowedIntegrationEvent
{
    public Guid FollowerId { get; set; }
    public Guid FollowedId { get; set; }
}