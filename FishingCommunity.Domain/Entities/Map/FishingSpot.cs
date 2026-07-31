using FishingCommunity.Domain.Common;
using FishingCommunity.Domain.Enums;
using FishingCommunity.Domain.Exceptions;

namespace FishingCommunity.Domain.Entities.Map;

public class FishingSpot : BaseAuditableEntity, IAggregateRoot
{
    public Guid CreatedByUserId { get; private set; }

    public string Name { get; private set; } = string.Empty;
    public string? Description { get; private set; }

    public double Latitude { get; private set; }
    public double Longitude { get; private set; }

    public FishingSpotType Type { get; private set; }

    // Community-driven: any user can propose a spot; unverified spots are still visible
    // but flagged so users know it hasn't been confirmed by an admin/expert yet.
    public bool IsVerified { get; private set; }

    private readonly List<string> _photoUrls = new();
    public IReadOnlyCollection<string> PhotoUrls => _photoUrls.AsReadOnly();

    private FishingSpot() { } // EF Core

    public FishingSpot(Guid createdByUserId, string name, double latitude, double longitude, FishingSpotType type, string? description = null)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new BusinessRuleValidationException("Fishing spot name is required.");
        }

        CreatedByUserId = createdByUserId;
        Name = name;
        Latitude = latitude;
        Longitude = longitude;
        Type = type;
        Description = description;
    }

    public void UpdateDetails(string name, string? description, FishingSpotType type)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new BusinessRuleValidationException("Fishing spot name is required.");
        }

        Name = name;
        Description = description;
        Type = type;
    }

    public void AddPhoto(string url) => _photoUrls.Add(url);
    public void RemovePhoto(string url) => _photoUrls.Remove(url);

    public void Verify() => IsVerified = true;
    public void Unverify() => IsVerified = false;
}