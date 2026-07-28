namespace FishingCommunity.Application.Features.Trips.Commands.CreateTrip;

public class CreateTripRequestDto
{
    public Guid BoatId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string LocationName { get; set; } = string.Empty;
    public double Latitude { get; set; }
    public double Longitude { get; set; }
    public DateTime DepartureDateTime { get; set; }
    public DateTime? EstimatedReturnDateTime { get; set; }
    public int Capacity { get; set; }
    public decimal PricePerPerson { get; set; }
}