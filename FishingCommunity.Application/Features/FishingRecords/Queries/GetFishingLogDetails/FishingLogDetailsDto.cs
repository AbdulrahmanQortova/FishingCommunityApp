namespace FishingCommunity.Application.Features.FishingRecords.Queries.GetFishingLogDetails;

public class FishingLogDetailsDto
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public string FishSpeciesName { get; set; } = string.Empty;
    public double? WeightKg { get; set; }
    public double? LengthCm { get; set; }
    public string? LocationName { get; set; }
    public double? Latitude { get; set; }
    public double? Longitude { get; set; }
    public string? Bait { get; set; }
    public string? Notes { get; set; }
    public DateTime CaughtDate { get; set; }
    public double? WeatherTemperature { get; set; }
    public string? WeatherDescription { get; set; }
    public List<string> PhotoUrls { get; set; } = new();
}