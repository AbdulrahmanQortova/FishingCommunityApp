using FishingCommunity.Domain.Common;
using FishingCommunity.Domain.Enums;
using FishingCommunity.Domain.Exceptions;

namespace FishingCommunity.Domain.Entities.Payments;

public class Payment : BaseAuditableEntity, IAggregateRoot
{
    public Guid OrderId { get; private set; }
    public Guid UserId { get; private set; }

    public PaymentMethod Method { get; private set; }
    public PaymentStatus Status { get; private set; }
    public decimal Amount { get; private set; }

    // Manual transfer specifics (InstaPay / Vodafone Cash)
    public string? SenderPhoneOrHandle { get; private set; } // The number/handle the user paid FROM
    public string? TransferProofUrl { get; private set; }
    public DateTime? ProofSubmittedDate { get; private set; }

    public Guid? ReviewedByAdminId { get; private set; }
    public DateTime? ReviewedDate { get; private set; }
    public string? RejectionReason { get; private set; }

    public DateTime? RefundedDate { get; private set; }

    private Payment() { } // EF Core

    public static Payment CreateCashOnDelivery(Guid orderId, Guid userId, decimal amount)
    {
        if (amount <= 0)
        {
            throw new BusinessRuleValidationException("Payment amount must be greater than zero.");
        }

        return new Payment
        {
            OrderId = orderId,
            UserId = userId,
            Method = PaymentMethod.CashOnDelivery,
            Status = PaymentStatus.PendingCollection,
            Amount = amount
        };
    }

    public static Payment CreateManualTransfer(Guid orderId, Guid userId, decimal amount, PaymentMethod method)
    {
        if (amount <= 0)
        {
            throw new BusinessRuleValidationException("Payment amount must be greater than zero.");
        }

        if (method == PaymentMethod.CashOnDelivery)
        {
            throw new BusinessRuleValidationException("Use CreateCashOnDelivery for cash-on-delivery payments.");
        }

        return new Payment
        {
            OrderId = orderId,
            UserId = userId,
            Method = method,
            Status = PaymentStatus.AwaitingProof,
            Amount = amount
        };
    }

    public void SubmitTransferProof(string senderPhoneOrHandle, string proofUrl)
    {
        if (Method == PaymentMethod.CashOnDelivery)
        {
            throw new BusinessRuleValidationException("Cannot submit transfer proof for a cash-on-delivery payment.");
        }

        if (Status != PaymentStatus.AwaitingProof)
        {
            throw new BusinessRuleValidationException("Transfer proof has already been submitted for this payment.");
        }

        SenderPhoneOrHandle = senderPhoneOrHandle;
        TransferProofUrl = proofUrl;
        ProofSubmittedDate = DateTime.UtcNow;
        Status = PaymentStatus.UnderReview;
    }

    public void Approve(Guid adminId)
    {
        if (Status != PaymentStatus.UnderReview)
        {
            throw new BusinessRuleValidationException("Only a payment under review can be approved.");
        }

        Status = PaymentStatus.Approved;
        ReviewedByAdminId = adminId;
        ReviewedDate = DateTime.UtcNow;
    }

    public void Reject(Guid adminId, string reason)
    {
        if (Status != PaymentStatus.UnderReview)
        {
            throw new BusinessRuleValidationException("Only a payment under review can be rejected.");
        }

        Status = PaymentStatus.Rejected;
        ReviewedByAdminId = adminId;
        ReviewedDate = DateTime.UtcNow;
        RejectionReason = reason;
    }

    public void MarkCollected()
    {
        // Cash on Delivery: confirmed collected upon delivery.
        if (Method != PaymentMethod.CashOnDelivery)
        {
            throw new BusinessRuleValidationException("Only cash-on-delivery payments can be marked as collected this way.");
        }

        if (Status != PaymentStatus.PendingCollection)
        {
            throw new BusinessRuleValidationException("This payment is not pending collection.");
        }

        Status = PaymentStatus.Approved;
        ReviewedDate = DateTime.UtcNow;
    }

    public void Refund()
    {
        if (Status != PaymentStatus.Approved)
        {
            throw new BusinessRuleValidationException("Only an approved payment can be refunded.");
        }

        Status = PaymentStatus.Refunded;
        RefundedDate = DateTime.UtcNow;
    }
}