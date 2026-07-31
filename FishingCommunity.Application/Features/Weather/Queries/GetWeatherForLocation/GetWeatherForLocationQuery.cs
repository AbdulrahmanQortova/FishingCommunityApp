using FishingCommunity.Shared.Wrappers;
using MediatR;

namespace FishingCommunity.Application.Features.Weather.Queries.GetWeatherForLocation;

public class GetWeatherForLocationQuery : IRequest<Result<WeatherResponseDto>>
{
    public double Latitude { get; set; }
    public double Longitude { get; set; }
}