namespace FishingCommunity.Application.Features.Shop.Checkout.Commands.Checkout;

public class CheckoutResponse
{
    public Guid OrderId { get; set; }
    public decimal SubtotalAmount { get; set; }
    public decimal DiscountAmount { get; set; }
    public decimal TotalAmount { get; set; }
}