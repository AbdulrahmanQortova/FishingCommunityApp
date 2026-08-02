namespace FishingCommunity.Application.Features.AiAssistant.Queries.GetEquipmentRecommendation;

public class EquipmentRecommendationDto
{
    public List<string> RecommendedRods { get; set; } = new();
    public List<string> RecommendedBait { get; set; } = new();
    public string? GeneralTips { get; set; }
}