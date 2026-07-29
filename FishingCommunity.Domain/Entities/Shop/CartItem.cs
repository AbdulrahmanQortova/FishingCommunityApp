using FishingCommunity.Domain.Common;
using FishingCommunity.Domain.Exceptions;

namespace FishingCommunity.Domain.Entities.Shop;

public class CartItem : BaseEntity
{
    public Guid CartId { get; private set; }
    public Cart Cart { get; private set; } = null!;

    public Guid ProductId { get; private set; }
    public Product Product { get; private set; } = null!;

    public int Quantity { get; private set; }

    // Price snapshot at the time the item was added — the actual charged price is
    // re-validated against the live Product price at checkout time regardless.
    public decimal UnitPriceSnapshot { get; private set; }

    private CartItem() { } // EF Core

    internal CartItem(Guid cartId, Guid productId, int quantity, decimal unitPriceSnapshot)
    {
        CartId = cartId;
        ProductId = productId;
        Quantity = quantity;
        UnitPriceSnapshot = unitPriceSnapshot;
    }

    internal void UpdateQuantity(int quantity)
    {
        if (quantity <= 0)
        {
            throw new BusinessRuleValidationException("Quantity must be greater than zero.");
        }

        Quantity = quantity;
    }
}