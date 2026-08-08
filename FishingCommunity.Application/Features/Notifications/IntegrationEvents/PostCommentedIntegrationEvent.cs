namespace FishingCommunity.Application.Features.Notifications.IntegrationEvents;

public class PostCommentedIntegrationEvent
{
    public Guid PostId { get; set; }
    public Guid CommenterId { get; set; }
    public Guid PostAuthorId { get; set; }
}