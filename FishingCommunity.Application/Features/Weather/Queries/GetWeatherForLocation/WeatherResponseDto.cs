using FishingCommunity.Application.Common.Models;

namespace FishingCommunity.Application.Features.Weather.Queries.GetWeatherForLocation;

public class WeatherResponseDto
{
    public double Temperature { get; set; }
    public double FeelsLike { get; set; }
    public double Humidity { get; set; }
    public double WindSpeed { get; set; }
    public double WindDirection { get; set; }
    public double? WaveHeight { get; set; }
    public string Description { get; set; } = string.Empty;
    public string IconCode { get; set; } = string.Empty;
    public DateTime RetrievedAt { get; set; }

    public FishingSuitabilityLevel FishingSuitabilityLevel { get; set; }
    public string FishingSuitabilityReason { get; set; } = string.Empty;
}