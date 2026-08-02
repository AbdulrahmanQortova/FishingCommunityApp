using FishingCommunity.Domain.Enums;

namespace FishingCommunity.Application.Features.Payments.Queries.GetMyPayments;

public class MyPaymentDto
{
    public Guid Id { get; set; }
    public Guid OrderId { get; set; }
    public PaymentMethod Method { get; set; }
    public PaymentStatus Status { get; set; }
    public decimal Amount { get; set; }
    public string? RejectionReason { get; set; }
    public DateTime CreatedDate { get; set; }
}