using FishingCommunity.Domain.Enums;

namespace FishingCommunity.Application.Features.Shop.Checkout.Commands.Checkout;

public class CheckoutRequestDto
{
    public Guid ShippingAddressId { get; set; }
    public string? CouponCode { get; set; }
    public PaymentMethod PaymentMethod { get; set; } = PaymentMethod.CashOnDelivery;
}