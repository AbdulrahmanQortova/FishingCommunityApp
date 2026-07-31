using FishingCommunity.Domain.Common;
using FishingCommunity.Domain.Exceptions;

namespace FishingCommunity.Domain.Entities.FishingRecords;

public class FishingLog : BaseAuditableEntity, IAggregateRoot
{
    public Guid UserId { get; private set; }

    public Guid FishSpeciesId { get; private set; }
    public FishSpecies FishSpecies { get; private set; } = null!;

    public double? WeightKg { get; private set; }
    public double? LengthCm { get; private set; }

    public string? LocationName { get; private set; }
    public double? Latitude { get; private set; }
    public double? Longitude { get; private set; }

    public string? Bait { get; private set; }
    public string? Notes { get; private set; }

    public DateTime CaughtDate { get; private set; }

    // Optional weather snapshot at the time of the catch — captured from the Weather
    // module at creation time, not a live reference (weather changes; the catch doesn't).
    public double? WeatherTemperature { get; private set; }
    public string? WeatherDescription { get; private set; }

    private readonly List<string> _photoUrls = new();
    public IReadOnlyCollection<string> PhotoUrls => _photoUrls.AsReadOnly();

    private FishingLog() { } // EF Core

    public FishingLog(
        Guid userId,
        Guid fishSpeciesId,
        DateTime caughtDate,
        double? weightKg = null,
        double? lengthCm = null,
        string? locationName = null,
        double? latitude = null,
        double? longitude = null,
        string? bait = null,
        string? notes = null)
    {
        if (caughtDate > DateTime.UtcNow)
        {
            throw new BusinessRuleValidationException("Catch date cannot be in the future.");
        }

        if (weightKg is < 0)
        {
            throw new BusinessRuleValidationException("Weight cannot be negative.");
        }

        if (lengthCm is < 0)
        {
            throw new BusinessRuleValidationException("Length cannot be negative.");
        }

        UserId = userId;
        FishSpeciesId = fishSpeciesId;
        CaughtDate = caughtDate;
        WeightKg = weightKg;
        LengthCm = lengthCm;
        LocationName = locationName;
        Latitude = latitude;
        Longitude = longitude;
        Bait = bait;
        Notes = notes;
    }

    public void UpdateDetails(
        double? weightKg, double? lengthCm, string? locationName,
        double? latitude, double? longitude, string? bait, string? notes)
    {
        if (weightKg is < 0)
        {
            throw new BusinessRuleValidationException("Weight cannot be negative.");
        }

        if (lengthCm is < 0)
        {
            throw new BusinessRuleValidationException("Length cannot be negative.");
        }

        WeightKg = weightKg;
        LengthCm = lengthCm;
        LocationName = locationName;
        Latitude = latitude;
        Longitude = longitude;
        Bait = bait;
        Notes = notes;
    }

    public void AttachWeatherSnapshot(double temperature, string description)
    {
        WeatherTemperature = temperature;
        WeatherDescription = description;
    }

    public void AddPhoto(string url) => _photoUrls.Add(url);
    public void RemovePhoto(string url) => _photoUrls.Remove(url);
}