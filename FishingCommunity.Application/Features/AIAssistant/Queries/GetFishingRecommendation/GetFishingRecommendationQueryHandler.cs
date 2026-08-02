using FishingCommunity.Application.Common.Interfaces;
using FishingCommunity.Shared.Wrappers;
using MediatR;

namespace FishingCommunity.Application.Features.AiAssistant.Queries.GetFishingRecommendation;

public class GetFishingRecommendationQueryHandler : IRequestHandler<GetFishingRecommendationQuery, Result<FishingRecommendationDto>>
{
    private readonly IAiAssistantService _aiAssistantService;
    private readonly IWeatherService _weatherService;

    public GetFishingRecommendationQueryHandler(IAiAssistantService aiAssistantService, IWeatherService weatherService)
    {
        _aiAssistantService = aiAssistantService;
        _weatherService = weatherService;
    }

    public async Task<Result<FishingRecommendationDto>> Handle(GetFishingRecommendationQuery request, CancellationToken cancellationToken)
    {
        // Combine live weather data with the AI recommendation — the recommendation
        // is only as good as the weather context feeding into it.
        var weather = await _weatherService.GetCurrentWeatherAsync(request.Latitude, request.Longitude, cancellationToken);

        var aiRequest = new FishingRecommendationRequest
        {
            Latitude = request.Latitude,
            Longitude = request.Longitude,
            WeatherTemperature = weather?.Temperature,
            WindSpeed = weather?.WindSpeed,
            WeatherDescription = weather?.Description,
            PreferredFishSpecies = request.PreferredFishSpecies
        };

        var aiResult = await _aiAssistantService.GetFishingRecommendationAsync(aiRequest, cancellationToken);

        var dto = new FishingRecommendationDto
        {
            Recommendation = aiResult.Recommendation,
            SuggestedSpecies = aiResult.SuggestedSpecies,
            BestTimeOfDay = aiResult.BestTimeOfDay,
            CurrentTemperature = weather?.Temperature,
            WeatherDescription = weather?.Description
        };

        return Result<FishingRecommendationDto>.Success(dto);
    }
}