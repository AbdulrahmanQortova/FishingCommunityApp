namespace FishingCommunity.Application.Features.Payments.Commands.SubmitTransferProof;

public class SubmitTransferProofRequestDto
{
    public string SenderPhoneOrHandle { get; set; } = string.Empty;
    public string ProofUrl { get; set; } = string.Empty;
}