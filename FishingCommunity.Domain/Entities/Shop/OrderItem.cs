using FishingCommunity.Domain.Common;

namespace FishingCommunity.Domain.Entities.Shop;

public class OrderItem : BaseEntity
{
    public Guid OrderId { get; private set; }
    public Order Order { get; private set; } = null!;

    public Guid ProductId { get; private set; }

    // Snapshot of the product's name and price at the time of purchase —
    // deliberately NOT a navigation property to the live Product. If the seller
    // later renames or reprices the product, historical orders must stay accurate.
    public string ProductName { get; private set; } = string.Empty;
    public int Quantity { get; private set; }
    public decimal UnitPrice { get; private set; }

    public decimal LineTotal => Quantity * UnitPrice;

    private OrderItem() { } // EF Core

    internal OrderItem(Guid orderId, Guid productId, string productName, int quantity, decimal unitPrice)
    {
        OrderId = orderId;
        ProductId = productId;
        ProductName = productName;
        Quantity = quantity;
        UnitPrice = unitPrice;
    }
}