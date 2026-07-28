using FishingCommunity.Domain.Enums;

namespace FishingCommunity.Application.Features.Trips.Queries.GetTripDetails;

public class TripDetailsDto
{
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string LocationName { get; set; } = string.Empty;
    public double Latitude { get; set; }
    public double Longitude { get; set; }
    public DateTime DepartureDateTime { get; set; }
    public DateTime? EstimatedReturnDateTime { get; set; }
    public int Capacity { get; set; }
    public int AvailableSeats { get; set; }
    public decimal PricePerPerson { get; set; }
    public TripStatus Status { get; set; }
    public double? AverageRating { get; set; }
    public IReadOnlyList<string> PhotoUrls { get; set; } = new List<string>();

    public Guid BoatId { get; set; }
    public string BoatName { get; set; } = string.Empty;

    public Guid OrganizerId { get; set; }

    public List<TripReviewDto> Reviews { get; set; } = new();
}

public class TripReviewDto
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public int Rating { get; set; }
    public string? Comment { get; set; }
    public DateTime CreatedDate { get; set; }
}