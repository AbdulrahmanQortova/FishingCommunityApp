using FishingCommunity.Application.Common.Interfaces;
using FishingCommunity.Application.Common.Models;

namespace FishingCommunity.Application.Common.Services;

public static class FishingSuitabilityCalculator
{
    public static FishingSuitability Calculate(WeatherResult weather)
    {
        // Simple heuristic rules — can be refined later with domain expert input
        // or replaced with a more sophisticated model.

        if (weather.WindSpeed > 12) // Strong wind (> ~43 km/h) — generally unsafe for small boats.
        {
            return new FishingSuitability
            {
                Level = FishingSuitabilityLevel.Poor,
                Reason = "Strong winds make fishing conditions unsafe."
            };
        }

        if (weather.WaveHeight is > 2.0)
        {
            return new FishingSuitability
            {
                Level = FishingSuitabilityLevel.Poor,
                Reason = "High waves make fishing conditions unsafe."
            };
        }

        if (weather.WindSpeed > 8 || weather.WaveHeight is > 1.2)
        {
            return new FishingSuitability
            {
                Level = FishingSuitabilityLevel.Fair,
                Reason = "Moderate wind/wave conditions — proceed with caution."
            };
        }

        if (weather.WindSpeed <= 4 && weather.Temperature is >= 15 and <= 32)
        {
            return new FishingSuitability
            {
                Level = FishingSuitabilityLevel.Excellent,
                Reason = "Calm winds and comfortable temperature — great conditions for fishing."
            };
        }

        return new FishingSuitability
        {
            Level = FishingSuitabilityLevel.Good,
            Reason = "Generally favorable conditions for fishing."
        };
    }
}