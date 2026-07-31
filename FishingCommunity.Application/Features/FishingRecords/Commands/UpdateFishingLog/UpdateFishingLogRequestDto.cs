namespace FishingCommunity.Application.Features.FishingRecords.Commands.UpdateFishingLog;

public class UpdateFishingLogRequestDto
{
    public double? WeightKg { get; set; }
    public double? LengthCm { get; set; }
    public string? LocationName { get; set; }
    public double? Latitude { get; set; }
    public double? Longitude { get; set; }
    public string? Bait { get; set; }
    public string? Notes { get; set; }
}