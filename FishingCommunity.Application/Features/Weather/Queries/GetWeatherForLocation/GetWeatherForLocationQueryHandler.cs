using FishingCommunity.Application.Common.Interfaces;
using FishingCommunity.Application.Common.Services;
using FishingCommunity.Shared.Wrappers;
using MediatR;

namespace FishingCommunity.Application.Features.Weather.Queries.GetWeatherForLocation;

public class GetWeatherForLocationQueryHandler : IRequestHandler<GetWeatherForLocationQuery, Result<WeatherResponseDto>>
{
    private readonly IWeatherService _weatherService;

    public GetWeatherForLocationQueryHandler(IWeatherService weatherService)
    {
        _weatherService = weatherService;
    }

    public async Task<Result<WeatherResponseDto>> Handle(GetWeatherForLocationQuery request, CancellationToken cancellationToken)
    {
        var weather = await _weatherService.GetCurrentWeatherAsync(request.Latitude, request.Longitude, cancellationToken);

        if (weather is null)
        {
            return Result<WeatherResponseDto>.Failure("Unable to retrieve weather data for this location right now. Please try again later.");
        }

        var suitability = FishingSuitabilityCalculator.Calculate(weather);

        var response = new WeatherResponseDto
        {
            Temperature = weather.Temperature,
            FeelsLike = weather.FeelsLike,
            Humidity = weather.Humidity,
            WindSpeed = weather.WindSpeed,
            WindDirection = weather.WindDirection,
            WaveHeight = weather.WaveHeight,
            Description = weather.Description,
            IconCode = weather.IconCode,
            RetrievedAt = weather.RetrievedAt,
            FishingSuitabilityLevel = suitability.Level,
            FishingSuitabilityReason = suitability.Reason
        };

        return Result<WeatherResponseDto>.Success(response);
    }
}