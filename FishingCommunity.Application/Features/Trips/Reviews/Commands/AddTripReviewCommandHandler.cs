using FishingCommunity.Domain.Entities.Trips;
using FishingCommunity.Domain.Exceptions;
using FishingCommunity.Domain.Interfaces;
using FishingCommunity.Shared.Wrappers;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace FishingCommunity.Application.Features.Trips.Reviews.Commands.AddTripReview;

public class AddTripReviewCommandHandler : IRequestHandler<AddTripReviewCommand, Result<AddTripReviewResponse>>
{
    private readonly IUnitOfWork _unitOfWork;

    public AddTripReviewCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<AddTripReviewResponse>> Handle(AddTripReviewCommand request, CancellationToken cancellationToken)
    {
        var trip = await _unitOfWork.Repository<Trip>().Query()
            .Where(t => t.Id == request.TripId)
            .Include(t => t.Bookings)
            .Include(t => t.Reviews)
            .FirstOrDefaultAsync(cancellationToken);

        if (trip is null)
        {
            throw new NotFoundException(nameof(Trip), request.TripId);
        }

        // Trip.AddReview() throws BusinessRuleValidationException if the trip isn't
        // completed, the user didn't participate, or they've already reviewed it.
        var review = trip.AddReview(request.UserId, request.Rating, request.Comment);

        _unitOfWork.Repository<Trip>().Update(trip);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        var response = new AddTripReviewResponse
        {
            ReviewId = review.Id,
            TripId = trip.Id,
            Rating = review.Rating,
            Comment = review.Comment
        };

        return Result<AddTripReviewResponse>.Success(response, "Review submitted successfully.");
    }
}