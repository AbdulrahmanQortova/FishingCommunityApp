using FishingCommunity.Domain.Enums;

namespace FishingCommunity.Application.Features.Admin.Reports.Commands.ResolveReport;

public class ResolveReportRequestDto
{
    public ReportStatus NewStatus { get; set; }
    public string? ResolutionNotes { get; set; }
    public bool DeletePost { get; set; }
}