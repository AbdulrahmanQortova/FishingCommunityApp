using FishingCommunity.Domain.Common;

namespace FishingCommunity.Domain.Entities.Community;

public class SavedPost : BaseEntity
{
    public Guid PostId { get; private set; }
    public Guid UserId { get; private set; }

    private SavedPost() { } // EF Core

    public SavedPost(Guid postId, Guid userId)
    {
        PostId = postId;
        UserId = userId;
    }
}