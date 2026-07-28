using FishingCommunity.Domain.Common;
using FishingCommunity.Domain.Enums;

namespace FishingCommunity.Domain.Entities.Community;

public class PostReport : BaseAuditableEntity
{
    public Guid PostId { get; private set; }
    public Post Post { get; private set; } = null!;

    public Guid ReportedByUserId { get; private set; }
    public ReportReason Reason { get; private set; }
    public string? AdditionalDetails { get; private set; }

    public ReportStatus Status { get; private set; } = ReportStatus.Pending;
    public string? ResolutionNotes { get; private set; }
    public Guid? ResolvedByUserId { get; private set; }
    public DateTime? ResolvedDate { get; private set; }

    private PostReport() { } // EF Core

    public PostReport(Guid postId, Guid reportedByUserId, ReportReason reason, string? additionalDetails = null)
    {
        PostId = postId;
        ReportedByUserId = reportedByUserId;
        Reason = reason;
        AdditionalDetails = additionalDetails;
    }

    public void Resolve(Guid resolvedByUserId, ReportStatus status, string? resolutionNotes)
    {
        Status = status;
        ResolvedByUserId = resolvedByUserId;
        ResolutionNotes = resolutionNotes;
        ResolvedDate = DateTime.UtcNow;
    }
}