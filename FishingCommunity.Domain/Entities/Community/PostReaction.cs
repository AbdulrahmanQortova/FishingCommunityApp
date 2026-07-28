using FishingCommunity.Domain.Common;
using FishingCommunity.Domain.Enums;

namespace FishingCommunity.Domain.Entities.Community;

public class PostReaction : BaseEntity
{
    public Guid PostId { get; private set; }
    public Post Post { get; private set; } = null!;

    public Guid UserId { get; private set; }
    public ReactionType Type { get; private set; }

    private PostReaction() { } // EF Core

    internal PostReaction(Guid postId, Guid userId, ReactionType type)
    {
        PostId = postId;
        UserId = userId;
        Type = type;
    }

    internal void ChangeType(ReactionType type)
    {
        Type = type;
    }
}