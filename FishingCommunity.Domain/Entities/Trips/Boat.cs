using FishingCommunity.Domain.Common;
using FishingCommunity.Domain.Enums;
using FishingCommunity.Domain.Events.Trips;

namespace FishingCommunity.Domain.Entities.Trips;

public class Boat : BaseAuditableEntity, IAggregateRoot
{
    public Guid OwnerId { get; private set; } // FK to ApplicationUser (BoatOwner role)

    public string Name { get; private set; } = string.Empty;
    public string? Description { get; private set; }
    public string RegistrationNumber { get; private set; } = string.Empty;
    public int Capacity { get; private set; }
    public string? MainPhotoUrl { get; private set; }

    public BoatStatus Status { get; private set; } = BoatStatus.Active;

    private readonly List<string> _photoUrls = new();
    public IReadOnlyCollection<string> PhotoUrls => _photoUrls.AsReadOnly();

    private readonly List<Trip> _trips = new();
    public IReadOnlyCollection<Trip> Trips => _trips.AsReadOnly();

    private Boat() { } // EF Core

    public Boat(Guid ownerId, string name, string registrationNumber, int capacity, string? description = null)
    {
        if (capacity <= 0)
        {
            throw new Domain.Exceptions.BusinessRuleValidationException("Boat capacity must be greater than zero.");
        }

        OwnerId = ownerId;
        Name = name;
        RegistrationNumber = registrationNumber;
        Capacity = capacity;
        Description = description;
    }

    public void UpdateDetails(string name, string? description, int capacity)
    {
        if (capacity <= 0)
        {
            throw new Domain.Exceptions.BusinessRuleValidationException("Boat capacity must be greater than zero.");
        }

        Name = name;
        Description = description;
        Capacity = capacity;
    }

    public void SetMainPhoto(string url)
    {
        MainPhotoUrl = url;
    }

    public void AddPhoto(string url)
    {
        _photoUrls.Add(url);
    }

    public void RemovePhoto(string url)
    {
        _photoUrls.Remove(url);
    }

    public void MarkUnderMaintenance()
    {
        Status = BoatStatus.UnderMaintenance;
    }

    public void Activate()
    {
        Status = BoatStatus.Active;
    }

    public void Deactivate()
    {
        Status = BoatStatus.Inactive;
    }
}