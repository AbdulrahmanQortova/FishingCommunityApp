namespace FishingCommunity.Application.Common.Interfaces;

public interface IWeatherService
{
    Task<WeatherResult?> GetCurrentWeatherAsync(double latitude, double longitude, CancellationToken cancellationToken = default);
}

public class WeatherResult
{
    public double Temperature { get; set; } // Celsius
    public double FeelsLike { get; set; }
    public double Humidity { get; set; } // Percentage
    public double WindSpeed { get; set; } // m/s
    public double WindDirection { get; set; } // Degrees
    public double? WaveHeight { get; set; } // Meters — not always available depending on provider/location
    public string Description { get; set; } = string.Empty; // e.g. "clear sky", "light rain"
    public string IconCode { get; set; } = string.Empty;
    public DateTime RetrievedAt { get; set; }
}