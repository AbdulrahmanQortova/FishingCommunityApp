using FishingCommunity.Domain.Enums;
using FishingCommunity.Shared.Wrappers;
using MediatR;

namespace FishingCommunity.Application.Features.Shop.Checkout.Commands.Checkout;

public class CheckoutCommand : IRequest<Result<CheckoutResponse>>
{
    public Guid UserId { get; set; }
    public Guid ShippingAddressId { get; set; }
    public string? CouponCode { get; set; }
    public PaymentMethod PaymentMethod { get; set; } = PaymentMethod.CashOnDelivery;
}