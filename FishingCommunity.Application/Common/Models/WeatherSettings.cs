namespace FishingCommunity.Application.Common.Models;

public class WeatherSettings
{
    public const string SectionName = "WeatherSettings";

    public string ApiKey { get; set; } = string.Empty;
    public string BaseUrl { get; set; } = "https://api.openweathermap.org/data/2.5";
    public int CacheDurationMinutes { get; set; } = 30;
}