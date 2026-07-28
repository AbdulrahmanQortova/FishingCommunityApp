using FluentValidation;

namespace FishingCommunity.Application.Features.Community.Posts.Commands.ReportPost;

public class ReportPostCommandValidator : AbstractValidator<ReportPostCommand>
{
    public ReportPostCommandValidator()
    {
        RuleFor(x => x.PostId).NotEmpty();
        RuleFor(x => x.ReportedByUserId).NotEmpty();
        RuleFor(x => x.Reason).IsInEnum();

        RuleFor(x => x.AdditionalDetails)
            .MaximumLength(1000)
            .When(x => x.AdditionalDetails is not null);
    }
}