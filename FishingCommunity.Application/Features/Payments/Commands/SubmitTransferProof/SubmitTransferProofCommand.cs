using FishingCommunity.Shared.Wrappers;
using MediatR;

namespace FishingCommunity.Application.Features.Payments.Commands.SubmitTransferProof;

public class SubmitTransferProofCommand : IRequest<Result>
{
    public Guid PaymentId { get; set; }
    public Guid RequestingUserId { get; set; }
    public string SenderPhoneOrHandle { get; set; } = string.Empty;
    public string ProofUrl { get; set; } = string.Empty;
}