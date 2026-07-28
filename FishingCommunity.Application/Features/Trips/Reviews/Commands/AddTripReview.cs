using FishingCommunity.Shared.Wrappers;
using MediatR;

namespace FishingCommunity.Application.Features.Trips.Reviews.Commands.AddTripReview;

public class AddTripReviewCommand : IRequest<Result<AddTripReviewResponse>>
{
    public Guid TripId { get; set; }
    public Guid UserId { get; set; }
    public int Rating { get; set; }
    public string? Comment { get; set; }
}