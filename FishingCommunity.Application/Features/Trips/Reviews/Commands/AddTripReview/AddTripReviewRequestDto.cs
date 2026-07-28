namespace FishingCommunity.Application.Features.Trips.Reviews.Commands.AddTripReview;

public class AddTripReviewRequestDto
{
    public int Rating { get; set; }
    public string? Comment { get; set; }
}