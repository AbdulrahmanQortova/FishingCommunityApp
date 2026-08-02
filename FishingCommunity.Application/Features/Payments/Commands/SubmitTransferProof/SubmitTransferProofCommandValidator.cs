using FluentValidation;

namespace FishingCommunity.Application.Features.Payments.Commands.SubmitTransferProof;

public class SubmitTransferProofCommandValidator : AbstractValidator<SubmitTransferProofCommand>
{
    public SubmitTransferProofCommandValidator()
    {
        RuleFor(x => x.PaymentId).NotEmpty();
        RuleFor(x => x.RequestingUserId).NotEmpty();

        RuleFor(x => x.SenderPhoneOrHandle)
            .NotEmpty().WithMessage("Please provide the phone number or handle you paid from.")
            .MaximumLength(50);

        RuleFor(x => x.ProofUrl)
            .NotEmpty().WithMessage("Transfer proof image is required.")
            .Must(url => Uri.IsWellFormedUriString(url, UriKind.Absolute))
            .WithMessage("Proof URL must be a valid, absolute URL.");
    }
}