using FishingCommunity.Domain.Common;

namespace FishingCommunity.Domain.Events.Shop;

public class OrderShippedEvent : DomainEvent
{
    public Guid OrderId { get; }
    public Guid UserId { get; }

    public OrderShippedEvent(Guid orderId, Guid userId)
    {
        OrderId = orderId;
        UserId = userId;
    }
}