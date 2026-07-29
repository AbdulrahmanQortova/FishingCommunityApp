// Features/Shop/Checkout/Commands/Checkout/CheckoutRequestDto.cs
namespace FishingCommunity.Application.Features.Shop.Checkout.Commands.Checkout;

public class CheckoutRequestDto
{
    public Guid ShippingAddressId { get; set; }
    public string? CouponCode { get; set; }
}