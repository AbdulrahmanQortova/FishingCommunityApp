using FishingCommunity.Domain.Common;
using FishingCommunity.Domain.Enums;
using FishingCommunity.Domain.Events.Shop;
using FishingCommunity.Domain.Exceptions;

namespace FishingCommunity.Domain.Entities.Shop;

public class Order : BaseAuditableEntity, IAggregateRoot
{
    public Guid UserId { get; private set; }

    public Guid ShippingAddressId { get; private set; }
    public ShippingAddress ShippingAddress { get; private set; } = null!;

    public string? CouponCode { get; private set; }
    public decimal DiscountAmount { get; private set; }
    public decimal SubtotalAmount { get; private set; }
    public decimal TotalAmount { get; private set; }

    public OrderStatus Status { get; private set; } = OrderStatus.Pending;

    public DateTime? ConfirmedDate { get; private set; }
    public DateTime? ShippedDate { get; private set; }
    public DateTime? DeliveredDate { get; private set; }
    public DateTime? CancelledDate { get; private set; }
    public string? CancellationReason { get; private set; }

    private readonly List<OrderItem> _items = new();
    public IReadOnlyCollection<OrderItem> Items => _items.AsReadOnly();

    private Order() { } // EF Core

    private Order(Guid userId, Guid shippingAddressId)
    {
        UserId = userId;
        ShippingAddressId = shippingAddressId;
    }

    /// <summary>
    /// Factory method — creates an order from a snapshot of cart items.
    /// Stock reservation must happen separately in the Application layer, since it
    /// requires loading each Product aggregate individually (Order doesn't own Products).
    /// </summary>
    public static Order CreateFromCartItems(
        Guid userId,
        Guid shippingAddressId,
        IEnumerable<(Guid ProductId, string ProductName, int Quantity, decimal UnitPrice)> items)
    {
        var itemsList = items.ToList();

        if (itemsList.Count == 0)
        {
            throw new BusinessRuleValidationException("Cannot create an order with no items.");
        }

        var order = new Order(userId, shippingAddressId);

        foreach (var item in itemsList)
        {
            order._items.Add(new OrderItem(order.Id, item.ProductId, item.ProductName, item.Quantity, item.UnitPrice));
        }

        order.SubtotalAmount = order._items.Sum(i => i.Quantity * i.UnitPrice);
        order.TotalAmount = order.SubtotalAmount;

        order.AddDomainEvent(new OrderCreatedEvent(order.Id, userId));

        return order;
    }

    public void ApplyCoupon(string couponCode, decimal discountAmount)
    {
        if (Status != OrderStatus.Pending)
        {
            throw new BusinessRuleValidationException("Coupons can only be applied to pending orders.");
        }

        CouponCode = couponCode;
        DiscountAmount = discountAmount;
        TotalAmount = Math.Max(0, SubtotalAmount - discountAmount);
    }

    public void Confirm()
    {
        if (Status != OrderStatus.Pending)
        {
            throw new BusinessRuleValidationException("Only a pending order can be confirmed.");
        }

        Status = OrderStatus.Confirmed;
        ConfirmedDate = DateTime.UtcNow;

        AddDomainEvent(new OrderConfirmedEvent(Id, UserId));
    }

    public void StartProcessing()
    {
        if (Status != OrderStatus.Confirmed)
        {
            throw new BusinessRuleValidationException("Only a confirmed order can start processing.");
        }

        Status = OrderStatus.Processing;
    }

    public void Ship()
    {
        if (Status != OrderStatus.Processing)
        {
            throw new BusinessRuleValidationException("Only an order in processing can be shipped.");
        }

        Status = OrderStatus.Shipped;
        ShippedDate = DateTime.UtcNow;

        AddDomainEvent(new OrderShippedEvent(Id, UserId));
    }

    public void Deliver()
    {
        if (Status != OrderStatus.Shipped)
        {
            throw new BusinessRuleValidationException("Only a shipped order can be marked as delivered.");
        }

        Status = OrderStatus.Delivered;
        DeliveredDate = DateTime.UtcNow;
    }

    public void Cancel(string? reason)
    {
        if (Status is OrderStatus.Delivered or OrderStatus.Cancelled or OrderStatus.Refunded)
        {
            throw new BusinessRuleValidationException("This order can no longer be cancelled.");
        }

        Status = OrderStatus.Cancelled;
        CancelledDate = DateTime.UtcNow;
        CancellationReason = reason;

        AddDomainEvent(new OrderCancelledEvent(Id, UserId));
    }

    public void Refund()
    {
        if (Status != OrderStatus.Delivered)
        {
            throw new BusinessRuleValidationException("Only a delivered order can be refunded.");
        }

        Status = OrderStatus.Refunded;
    }
}