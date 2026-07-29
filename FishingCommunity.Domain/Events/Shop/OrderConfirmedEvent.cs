using FishingCommunity.Domain.Common;

namespace FishingCommunity.Domain.Events.Shop;

public class OrderConfirmedEvent : DomainEvent
{
    public Guid OrderId { get; }
    public Guid UserId { get; }

    public OrderConfirmedEvent(Guid orderId, Guid userId)
    {
        OrderId = orderId;
        UserId = userId;
    }
}