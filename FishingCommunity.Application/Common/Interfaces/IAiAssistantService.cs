namespace FishingCommunity.Application.Common.Interfaces;

public interface IAiAssistantService
{
    Task<FishingRecommendationResult> GetFishingRecommendationAsync(FishingRecommendationRequest request, CancellationToken cancellationToken = default);
    Task<EquipmentRecommendationResult> GetEquipmentRecommendationAsync(EquipmentRecommendationRequest request, CancellationToken cancellationToken = default);
}

public class FishingRecommendationRequest
{
    public double Latitude { get; set; }
    public double Longitude { get; set; }
    public double? WeatherTemperature { get; set; }
    public double? WindSpeed { get; set; }
    public string? WeatherDescription { get; set; }
    public string? PreferredFishSpecies { get; set; } // Optional — user's target species
}

public class FishingRecommendationResult
{
    public string Recommendation { get; set; } = string.Empty;
    public List<string> SuggestedSpecies { get; set; } = new();
    public string? BestTimeOfDay { get; set; }
}

public class EquipmentRecommendationRequest
{
    public string TargetSpecies { get; set; } = string.Empty;
    public string? FishingSpotType { get; set; } // e.g. "DeepSea", "Lake" — matches FishingSpotType
    public string ExperienceLevel { get; set; } = "Beginner"; // Beginner, Intermediate, Advanced
}

public class EquipmentRecommendationResult
{
    public List<string> RecommendedRods { get; set; } = new();
    public List<string> RecommendedBait { get; set; } = new();
    public string? GeneralTips { get; set; }
}