namespace FishingCommunity.Application.Features.Shop.Wishlist.Queries.GetMyWishlist;

public class WishlistItemDto
{
    public Guid ProductId { get; set; }
    public string ProductName { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public string? MainPhotoUrl { get; set; }
    public bool InStock { get; set; }
}