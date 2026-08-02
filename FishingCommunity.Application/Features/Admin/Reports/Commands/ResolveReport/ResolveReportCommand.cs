using FishingCommunity.Domain.Enums;
using FishingCommunity.Shared.Wrappers;
using MediatR;

namespace FishingCommunity.Application.Features.Admin.Reports.Commands.ResolveReport;

public class ResolveReportCommand : IRequest<Result>
{
    public Guid ReportId { get; set; }
    public Guid AdminUserId { get; set; }
    public ReportStatus NewStatus { get; set; } // ActionTaken or Dismissed
    public string? ResolutionNotes { get; set; }
    public bool DeletePost { get; set; } // If true and NewStatus == ActionTaken, deletes the reported post too.
}