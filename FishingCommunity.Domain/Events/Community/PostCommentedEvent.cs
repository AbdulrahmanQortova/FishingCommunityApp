using FishingCommunity.Domain.Common;

namespace FishingCommunity.Domain.Events.Community;

public class PostCommentedEvent : DomainEvent
{
    public Guid PostId { get; }
    public Guid CommentId { get; }
    public Guid CommenterId { get; }
    public Guid PostAuthorId { get; }

    public PostCommentedEvent(Guid postId, Guid commentId, Guid commenterId, Guid postAuthorId)
    {
        PostId = postId;
        CommentId = commentId;
        CommenterId = commenterId;
        PostAuthorId = postAuthorId;
    }
}