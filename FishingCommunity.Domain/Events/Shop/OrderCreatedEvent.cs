using FishingCommunity.Domain.Common;

namespace FishingCommunity.Domain.Events.Shop;

public class OrderCreatedEvent : DomainEvent
{
    public Guid OrderId { get; }
    public Guid UserId { get; }

    public OrderCreatedEvent(Guid orderId, Guid userId)
    {
        OrderId = orderId;
        UserId = userId;
    }
}