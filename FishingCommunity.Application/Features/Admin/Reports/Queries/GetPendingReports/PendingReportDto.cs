using FishingCommunity.Domain.Enums;

namespace FishingCommunity.Application.Features.Admin.Reports.Queries.GetPendingReports;

public class PendingReportDto
{
    public Guid Id { get; set; }
    public Guid PostId { get; set; }
    public Guid ReportedByUserId { get; set; }
    public ReportReason Reason { get; set; }
    public string? AdditionalDetails { get; set; }
    public DateTime CreatedDate { get; set; }
}