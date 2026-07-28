using FishingCommunity.Application.Common.Extensions;
using FishingCommunity.Domain.Entities.Trips;
using FishingCommunity.Domain.Enums;
using FishingCommunity.Domain.Interfaces;
using FishingCommunity.Shared.Wrappers;
using MediatR;

namespace FishingCommunity.Application.Features.Trips.Queries.GetUpcomingTrips;

public class GetUpcomingTripsQueryHandler : IRequestHandler<GetUpcomingTripsQuery, Result<PaginatedList<TripSummaryDto>>>
{
    private readonly IUnitOfWork _unitOfWork;

    public GetUpcomingTripsQueryHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<PaginatedList<TripSummaryDto>>> Handle(GetUpcomingTripsQuery request, CancellationToken cancellationToken)
    {
        var query = _unitOfWork.Repository<Trip>().Query()
            .Where(t => t.Status == TripStatus.Scheduled && t.DepartureDateTime > DateTime.UtcNow);

        if (!string.IsNullOrWhiteSpace(request.LocationName))
        {
            query = query.Where(t => t.LocationName.Contains(request.LocationName));
        }

        if (request.MaxPrice.HasValue)
        {
            query = query.Where(t => t.PricePerPerson <= request.MaxPrice.Value);
        }

        if (request.DepartureAfter.HasValue)
        {
            query = query.Where(t => t.DepartureDateTime >= request.DepartureAfter.Value);
        }

        var projectedQuery = query
            .OrderBy(t => t.DepartureDateTime)
            .Select(t => new TripSummaryDto
            {
                Id = t.Id,
                Title = t.Title,
                LocationName = t.LocationName,
                DepartureDateTime = t.DepartureDateTime,
                AvailableSeats = t.Capacity - t.Bookings.Count(b =>
                    b.Status == BookingStatus.Approved ||
                    b.Status == BookingStatus.CheckedIn ||
                    b.Status == BookingStatus.Completed),
                PricePerPerson = t.PricePerPerson,
                AverageRating = t.Reviews.Any() ? t.Reviews.Average(r => r.Rating) : null,
                BoatName = t.Boat.Name,
                MainPhotoUrl = t.Boat.MainPhotoUrl
            });

        var paginatedResult = await projectedQuery.ToPaginatedListAsync(request.PageNumber, request.PageSize, cancellationToken);

        return Result<PaginatedList<TripSummaryDto>>.Success(paginatedResult);
    }
}