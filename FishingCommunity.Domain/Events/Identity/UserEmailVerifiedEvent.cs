using FishingCommunity.Domain.Common;

namespace FishingCommunity.Domain.Events.Identity;

public class UserEmailVerifiedEvent : DomainEvent
{
    public Guid UserId { get; }

    public UserEmailVerifiedEvent(Guid userId)
    {
        UserId = userId;
    }
}