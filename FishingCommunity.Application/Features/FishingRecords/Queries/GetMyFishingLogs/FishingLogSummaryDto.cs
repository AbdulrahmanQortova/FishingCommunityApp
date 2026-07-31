namespace FishingCommunity.Application.Features.FishingRecords.Queries.GetMyFishingLogs;

public class FishingLogSummaryDto
{
    public Guid Id { get; set; }
    public string FishSpeciesName { get; set; } = string.Empty;
    public double? WeightKg { get; set; }
    public double? LengthCm { get; set; }
    public string? LocationName { get; set; }
    public DateTime CaughtDate { get; set; }
    public string? MainPhotoUrl { get; set; }
}