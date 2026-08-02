namespace FishingCommunity.Application.Features.AiAssistant.Queries.GetFishingRecommendation;

public class FishingRecommendationDto
{
    public string Recommendation { get; set; } = string.Empty;
    public List<string> SuggestedSpecies { get; set; } = new();
    public string? BestTimeOfDay { get; set; }
    public double? CurrentTemperature { get; set; }
    public string? WeatherDescription { get; set; }
}