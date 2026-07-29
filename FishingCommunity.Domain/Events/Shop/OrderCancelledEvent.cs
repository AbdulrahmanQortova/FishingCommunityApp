using FishingCommunity.Domain.Common;

namespace FishingCommunity.Domain.Events.Shop;

public class OrderCancelledEvent : DomainEvent
{
    public Guid OrderId { get; }
    public Guid UserId { get; }

    public OrderCancelledEvent(Guid orderId, Guid userId)
    {
        OrderId = orderId;
        UserId = userId;
    }
}