namespace FishingCommunity.Application.Features.Notifications.IntegrationEvents;

public class PostLikedIntegrationEvent
{
    public Guid PostId { get; set; }
    public Guid LikedByUserId { get; set; }
    public Guid PostAuthorId { get; set; }
}