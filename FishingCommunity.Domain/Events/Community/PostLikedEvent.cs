using FishingCommunity.Domain.Common;

namespace FishingCommunity.Domain.Events.Community;

public class PostLikedEvent : DomainEvent
{
    public Guid PostId { get; }
    public Guid LikedByUserId { get; }
    public Guid PostAuthorId { get; }

    public PostLikedEvent(Guid postId, Guid likedByUserId, Guid postAuthorId)
    {
        PostId = postId;
        LikedByUserId = likedByUserId;
        PostAuthorId = postAuthorId;
    }
}