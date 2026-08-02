using FishingCommunity.Application.Common.Interfaces;
using FishingCommunity.Shared.Wrappers;
using MediatR;

namespace FishingCommunity.Application.Features.AiAssistant.Queries.GetEquipmentRecommendation;

public class GetEquipmentRecommendationQueryHandler : IRequestHandler<GetEquipmentRecommendationQuery, Result<EquipmentRecommendationDto>>
{
    private readonly IAiAssistantService _aiAssistantService;

    public GetEquipmentRecommendationQueryHandler(IAiAssistantService aiAssistantService)
    {
        _aiAssistantService = aiAssistantService;
    }

    public async Task<Result<EquipmentRecommendationDto>> Handle(GetEquipmentRecommendationQuery request, CancellationToken cancellationToken)
    {
        var aiRequest = new EquipmentRecommendationRequest
        {
            TargetSpecies = request.TargetSpecies,
            FishingSpotType = request.FishingSpotType,
            ExperienceLevel = request.ExperienceLevel
        };

        var result = await _aiAssistantService.GetEquipmentRecommendationAsync(aiRequest, cancellationToken);

        var dto = new EquipmentRecommendationDto
        {
            RecommendedRods = result.RecommendedRods,
            RecommendedBait = result.RecommendedBait,
            GeneralTips = result.GeneralTips
        };

        return Result<EquipmentRecommendationDto>.Success(dto);
    }
}