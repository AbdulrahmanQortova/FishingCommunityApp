using FishingCommunity.Domain.Common;

namespace FishingCommunity.Domain.Events.Community;

public class UserFollowedEvent : DomainEvent
{
    public Guid FollowerId { get; }
    public Guid FollowedId { get; }

    public UserFollowedEvent(Guid followerId, Guid followedId)
    {
        FollowerId = followerId;
        FollowedId = followedId;
    }
}