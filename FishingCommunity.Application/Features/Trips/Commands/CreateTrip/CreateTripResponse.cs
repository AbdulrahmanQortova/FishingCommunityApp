namespace FishingCommunity.Application.Features.Trips.Commands.CreateTrip;

public class CreateTripResponse
{
    public Guid TripId { get; set; }
    public string Title { get; set; } = string.Empty;
    public DateTime DepartureDateTime { get; set; }
    public int Capacity { get; set; }
}