namespace FishingCommunity.Domain.Enums;

public enum PaymentStatus
{
    // Cash on Delivery starts and stays here until the order is delivered.
    PendingCollection = 1,

    // Manual transfer: awaiting the user to submit proof, then awaiting admin review.
    AwaitingProof = 2,
    UnderReview = 3,

    Approved = 4,
    Rejected = 5,
    Refunded = 6
}