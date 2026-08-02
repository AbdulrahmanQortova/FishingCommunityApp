using FishingCommunity.Domain.Enums;

namespace FishingCommunity.Application.Features.Payments.Queries.GetPendingPaymentsForReview;

public class PendingPaymentDto
{
    public Guid Id { get; set; }
    public Guid OrderId { get; set; }
    public Guid UserId { get; set; }
    public PaymentMethod Method { get; set; }
    public decimal Amount { get; set; }
    public string? SenderPhoneOrHandle { get; set; }
    public string? TransferProofUrl { get; set; }
    public DateTime? ProofSubmittedDate { get; set; }
}