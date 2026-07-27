namespace FishingCommunity.Application.Features.Trips.Boats.Commands.CreateBoat;

public class CreateBoatResponse
{
    public Guid BoatId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string RegistrationNumber { get; set; } = string.Empty;
    public int Capacity { get; set; }
}