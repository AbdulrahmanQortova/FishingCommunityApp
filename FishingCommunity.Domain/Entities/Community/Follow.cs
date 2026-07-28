using FishingCommunity.Domain.Common;
using FishingCommunity.Domain.Exceptions;

namespace FishingCommunity.Domain.Entities.Community;

public class Follow : BaseEntity
{
    public Guid FollowerId { get; private set; }  // The user who is following
    public Guid FollowedId { get; private set; }  // The user being followed

    private Follow() { } // EF Core

    public Follow(Guid followerId, Guid followedId)
    {
        if (followerId == followedId)
        {
            throw new BusinessRuleValidationException("You cannot follow yourself.");
        }

        FollowerId = followerId;
        FollowedId = followedId;
    }
}