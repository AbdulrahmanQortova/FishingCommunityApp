namespace FishingCommunity.Application.Features.Trips.Queries.GetUpcomingTrips;

public class TripSummaryDto
{
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string LocationName { get; set; } = string.Empty;
    public DateTime DepartureDateTime { get; set; }
    public int AvailableSeats { get; set; }
    public decimal PricePerPerson { get; set; }
    public double? AverageRating { get; set; }
    public string BoatName { get; set; } = string.Empty;
    public string? MainPhotoUrl { get; set; }
}