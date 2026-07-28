namespace FishingCommunity.Application.Features.Trips.Boats.Commands.CreateBoat;

public class CreateBoatRequestDto
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string RegistrationNumber { get; set; } = string.Empty;
    public int Capacity { get; set; }
}