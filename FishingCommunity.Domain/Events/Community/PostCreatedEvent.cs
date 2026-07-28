using FishingCommunity.Domain.Common;

namespace FishingCommunity.Domain.Events.Community;

public class PostCreatedEvent : DomainEvent
{
    public Guid PostId { get; }
    public Guid AuthorId { get; }

    public PostCreatedEvent(Guid postId, Guid authorId)
    {
        PostId = postId;
        AuthorId = authorId;
    }
}