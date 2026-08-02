using FishingCommunity.Shared.Wrappers;
using MediatR;

namespace FishingCommunity.Application.Features.AiAssistant.Queries.GetFishingRecommendation;

public class GetFishingRecommendationQuery : IRequest<Result<FishingRecommendationDto>>
{
    public double Latitude { get; set; }
    public double Longitude { get; set; }
    public string? PreferredFishSpecies { get; set; }
}