using FishingCommunity.Domain.Enums;

namespace FishingCommunity.Application.Features.Shop.Checkout.Commands.Checkout;

public class CheckoutResponse
{
    public Guid OrderId { get; set; }
    public Guid PaymentId { get; set; }
    public PaymentMethod PaymentMethod { get; set; }
    public PaymentStatus PaymentStatus { get; set; }
    public decimal SubtotalAmount { get; set; }
    public decimal DiscountAmount { get; set; }
    public decimal TotalAmount { get; set; }
}