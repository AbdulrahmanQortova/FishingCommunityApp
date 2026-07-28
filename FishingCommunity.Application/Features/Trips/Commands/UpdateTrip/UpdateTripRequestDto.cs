namespace FishingCommunity.Application.Features.Trips.Commands.UpdateTrip;

public class UpdateTripRequestDto
{
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public DateTime DepartureDateTime { get; set; }
    public DateTime? EstimatedReturnDateTime { get; set; }
    public decimal PricePerPerson { get; set; }
}