using FishingCommunity.Shared.Wrappers;
using MediatR;

namespace FishingCommunity.Application.Features.Shop.Wishlist.Queries.GetMyWishlist;

public class GetMyWishlistQuery : IRequest<Result<List<WishlistItemDto>>>
{
    public Guid UserId { get; set; }
}