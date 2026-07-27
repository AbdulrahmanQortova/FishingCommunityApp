using FishingCommunity.Domain.Common;

namespace FishingCommunity.Domain.Events.Identity;

public class UserRegisteredEvent : DomainEvent
{
    public Guid UserId { get; }
    public string Email { get; }

    public UserRegisteredEvent(Guid userId, string email)
    {
        UserId = userId;
        Email = email;
    }
}