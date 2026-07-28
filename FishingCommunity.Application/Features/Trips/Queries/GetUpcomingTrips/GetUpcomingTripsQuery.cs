using FishingCommunity.Shared.Wrappers;
using MediatR;

namespace FishingCommunity.Application.Features.Trips.Queries.GetUpcomingTrips;

public class GetUpcomingTripsQuery : IRequest<Result<PaginatedList<TripSummaryDto>>>
{
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 10;

    // Optional filters
    public string? LocationName { get; set; }
    public decimal? MaxPrice { get; set; }
    public DateTime? DepartureAfter { get; set; }
}