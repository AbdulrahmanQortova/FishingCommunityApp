namespace FishingCommunity.Application.Features.Trips.Reviews.Commands.AddTripReview;

public class AddTripReviewResponse
{
    public Guid ReviewId { get; set; }
    public Guid TripId { get; set; }
    public int Rating { get; set; }
    public string? Comment { get; set; }
}