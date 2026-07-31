using System.Text.Json;
using FishingCommunity.Application.Common.Interfaces;
using FishingCommunity.Application.Common.Models;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace FishingCommunity.Infrastructure.Services.Weather;

public class WeatherService : IWeatherService
{
    private readonly HttpClient _httpClient;
    private readonly WeatherSettings _settings;
    private readonly IDistributedCache _cache;
    private readonly ILogger<WeatherService> _logger;

    public WeatherService(
        HttpClient httpClient,
        IOptions<WeatherSettings> settings,
        IDistributedCache cache,
        ILogger<WeatherService> logger)
    {
        _httpClient = httpClient;
        _settings = settings.Value;
        _cache = cache;
        _logger = logger;
    }

    public async Task<WeatherResult?> GetCurrentWeatherAsync(double latitude, double longitude, CancellationToken cancellationToken = default)
    {
        var cacheKey = BuildCacheKey(latitude, longitude);

        var cached = await _cache.GetStringAsync(cacheKey, cancellationToken);
        if (cached is not null)
        {
            return JsonSerializer.Deserialize<WeatherResult>(cached);
        }

        var result = await FetchFromProviderAsync(latitude, longitude, cancellationToken);

        if (result is not null)
        {
            var options = new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(_settings.CacheDurationMinutes)
            };

            await _cache.SetStringAsync(cacheKey, JsonSerializer.Serialize(result), options, cancellationToken);
        }

        return result;
    }

    private async Task<WeatherResult?> FetchFromProviderAsync(double latitude, double longitude, CancellationToken cancellationToken)
    {
        try
        {
            var url = $"{_settings.BaseUrl}/weather?lat={latitude}&lon={longitude}&units=metric&appid={_settings.ApiKey}";

            using var response = await _httpClient.GetAsync(url, cancellationToken);

            if (!response.IsSuccessStatusCode)
            {
                _logger.LogWarning("Weather provider returned {StatusCode} for ({Lat}, {Lon})", response.StatusCode, latitude, longitude);
                return null;
            }

            await using var stream = await response.Content.ReadAsStreamAsync(cancellationToken);
            var payload = await JsonSerializer.DeserializeAsync<OpenWeatherMapResponse>(stream, JsonOptions, cancellationToken);

            if (payload is null)
            {
                return null;
            }

            return new WeatherResult
            {
                Temperature = payload.Main.Temp,
                FeelsLike = payload.Main.FeelsLike,
                Humidity = payload.Main.Humidity,
                WindSpeed = payload.Wind.Speed,
                WindDirection = payload.Wind.Deg,
                // OpenWeatherMap's free tier doesn't include marine/wave data — left null.
                // A dedicated marine API (e.g. Stormglass.io) would be needed for real wave height.
                WaveHeight = null,
                Description = payload.Weather.FirstOrDefault()?.Description ?? "N/A",
                IconCode = payload.Weather.FirstOrDefault()?.Icon ?? string.Empty,
                RetrievedAt = DateTime.UtcNow
            };
        }
        catch (Exception ex)
        {
            // Network failure, timeout, malformed response, etc. — treated as "unavailable",
            // not a hard error, consistent with how the Application layer handles a null result.
            _logger.LogError(ex, "Failed to fetch weather data for ({Lat}, {Lon})", latitude, longitude);
            return null;
        }
    }

    private static string BuildCacheKey(double latitude, double longitude)
    {
        // Round to 2 decimal places (~1.1km precision) so nearby requests share the same cache entry.
        var roundedLat = Math.Round(latitude, 2);
        var roundedLon = Math.Round(longitude, 2);
        return FishingCommunity.Shared.Constants.CacheKeys.Weather($"{roundedLat}_{roundedLon}");
    }

    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };

    // --- Internal DTOs matching OpenWeatherMap's JSON response shape ---
    private class OpenWeatherMapResponse
    {
        public MainData Main { get; set; } = new();
        public WindData Wind { get; set; } = new();
        public List<WeatherData> Weather { get; set; } = new();
    }

    private class MainData
    {
        public double Temp { get; set; }

        [System.Text.Json.Serialization.JsonPropertyName("feels_like")]
        public double FeelsLike { get; set; }

        public double Humidity { get; set; }
    }

    private class WindData
    {
        public double Speed { get; set; }
        public double Deg { get; set; }
    }

    private class WeatherData
    {
        public string Description { get; set; } = string.Empty;
        public string Icon { get; set; } = string.Empty;
    }
}