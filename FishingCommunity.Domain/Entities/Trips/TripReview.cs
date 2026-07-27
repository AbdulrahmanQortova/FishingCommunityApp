using FishingCommunity.Domain.Common;
using FishingCommunity.Domain.Exceptions;

namespace FishingCommunity.Domain.Entities.Trips;

public class TripReview : BaseAuditableEntity
{
    public Guid TripId { get; private set; }
    public Trip Trip { get; private set; } = null!;

    public Guid UserId { get; private set; }
    public int Rating { get; private set; } // 1-5
    public string? Comment { get; private set; }

    private TripReview() { } // EF Core

    internal TripReview(Guid tripId, Guid userId, int rating, string? comment)
    {
        if (rating is < 1 or > 5)
        {
            throw new BusinessRuleValidationException("Rating must be between 1 and 5.");
        }

        TripId = tripId;
        UserId = userId;
        Rating = rating;
        Comment = comment;
    }
}