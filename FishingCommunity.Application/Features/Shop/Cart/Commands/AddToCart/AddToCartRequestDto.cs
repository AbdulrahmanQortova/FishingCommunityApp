// Features/Shop/Cart/Commands/AddToCart/AddToCartRequestDto.cs
namespace FishingCommunity.Application.Features.Shop.Cart.Commands.AddToCart;

public class AddToCartRequestDto
{
    public Guid ProductId { get; set; }
    public int Quantity { get; set; } = 1;
}