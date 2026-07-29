using FishingCommunity.Domain.Common;

namespace FishingCommunity.Domain.Entities.Shop;

public class WishlistItem : BaseEntity
{
    public Guid UserId { get; private set; }
    public Guid ProductId { get; private set; }
    public Product Product { get; private set; } = null!;

    private WishlistItem() { } // EF Core

    public WishlistItem(Guid userId, Guid productId)
    {
        UserId = userId;
        ProductId = productId;
    }
}