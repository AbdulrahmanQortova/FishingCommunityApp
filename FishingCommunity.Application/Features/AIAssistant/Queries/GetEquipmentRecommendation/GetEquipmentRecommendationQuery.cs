using FishingCommunity.Shared.Wrappers;
using MediatR;

namespace FishingCommunity.Application.Features.AiAssistant.Queries.GetEquipmentRecommendation;

public class GetEquipmentRecommendationQuery : IRequest<Result<EquipmentRecommendationDto>>
{
    public string TargetSpecies { get; set; } = string.Empty;
    public string? FishingSpotType { get; set; }
    public string ExperienceLevel { get; set; } = "Beginner";
}