using FluentValidation;

namespace FishingCommunity.Application.Features.Admin.Reports.Commands.ResolveReport;

public class ResolveReportCommandValidator : AbstractValidator<ResolveReportCommand>
{
    public ResolveReportCommandValidator()
    {
        RuleFor(x => x.ReportId).NotEmpty();
        RuleFor(x => x.AdminUserId).NotEmpty();

        RuleFor(x => x.NewStatus)
            .Must(s => s == Domain.Enums.ReportStatus.ActionTaken || s == Domain.Enums.ReportStatus.Dismissed)
            .WithMessage("New status must be either ActionTaken or Dismissed.");

        RuleFor(x => x.ResolutionNotes)
            .MaximumLength(1000)
            .When(x => x.ResolutionNotes is not null);
    }
}