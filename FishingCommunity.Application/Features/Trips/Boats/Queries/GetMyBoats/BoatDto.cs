using FishingCommunity.Domain.Enums;

namespace FishingCommunity.Application.Features.Trips.Boats.Queries.GetMyBoats;

public class BoatDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string RegistrationNumber { get; set; } = string.Empty;
    public int Capacity { get; set; }
    public string? MainPhotoUrl { get; set; }
    public BoatStatus Status { get; set; }
}