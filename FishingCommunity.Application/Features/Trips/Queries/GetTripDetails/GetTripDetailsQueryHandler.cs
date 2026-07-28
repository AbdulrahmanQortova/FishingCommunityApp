using FishingCommunity.Domain.Entities.Trips;
using FishingCommunity.Domain.Interfaces;
using FishingCommunity.Shared.Wrappers;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace FishingCommunity.Application.Features.Trips.Queries.GetTripDetails;

public class GetTripDetailsQueryHandler : IRequestHandler<GetTripDetailsQuery, Result<TripDetailsDto>>
{
    private readonly IUnitOfWork _unitOfWork;

    public GetTripDetailsQueryHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<TripDetailsDto>> Handle(GetTripDetailsQuery request, CancellationToken cancellationToken)
    {
        var trip = await _unitOfWork.Repository<Trip>().Query()
            .Where(t => t.Id == request.TripId)
            .Include(t => t.Boat)
            .Include(t => t.Bookings)
            .Include(t => t.Reviews)
            .FirstOrDefaultAsync(cancellationToken);

        if (trip is null)
        {
            return Result<TripDetailsDto>.Failure("Trip not found.");
        }

        var dto = new TripDetailsDto
        {
            Id = trip.Id,
            Title = trip.Title,
            Description = trip.Description,
            LocationName = trip.LocationName,
            Latitude = trip.Latitude,
            Longitude = trip.Longitude,
            DepartureDateTime = trip.DepartureDateTime,
            EstimatedReturnDateTime = trip.EstimatedReturnDateTime,
            Capacity = trip.Capacity,
            AvailableSeats = trip.AvailableSeats,
            PricePerPerson = trip.PricePerPerson,
            Status = trip.Status,
            AverageRating = trip.AverageRating,
            PhotoUrls = trip.PhotoUrls.ToList(),
            BoatId = trip.BoatId,
            BoatName = trip.Boat.Name,
            OrganizerId = trip.OrganizerId,
            Reviews = trip.Reviews.Select(r => new TripReviewDto
            {
                Id = r.Id,
                UserId = r.UserId,
                Rating = r.Rating,
                Comment = r.Comment,
                CreatedDate = r.CreatedDate
            }).ToList()
        };

        return Result<TripDetailsDto>.Success(dto);
    }
}