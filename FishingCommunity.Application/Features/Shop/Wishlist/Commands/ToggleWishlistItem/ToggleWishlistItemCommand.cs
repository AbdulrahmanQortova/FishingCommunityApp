using FishingCommunity.Shared.Wrappers;
using MediatR;

namespace FishingCommunity.Application.Features.Shop.Wishlist.Commands.ToggleWishlistItem;

public class ToggleWishlistItemCommand : IRequest<Result<bool>> // true = added, false = removed
{
    public Guid UserId { get; set; }
    public Guid ProductId { get; set; }
}