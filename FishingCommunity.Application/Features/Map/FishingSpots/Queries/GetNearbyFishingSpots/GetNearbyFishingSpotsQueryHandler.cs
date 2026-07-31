using FishingCommunity.Domain.Entities.Map;
using FishingCommunity.Domain.Interfaces;
using FishingCommunity.Shared.Utilities;
using FishingCommunity.Shared.Wrappers;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace FishingCommunity.Application.Features.Map.FishingSpots.Queries.GetNearbyFishingSpots;

public class GetNearbyFishingSpotsQueryHandler : IRequestHandler<GetNearbyFishingSpotsQuery, Result<List<FishingSpotDto>>>
{
    private readonly IUnitOfWork _unitOfWork;

    public GetNearbyFishingSpotsQueryHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<List<FishingSpotDto>>> Handle(GetNearbyFishingSpotsQuery request, CancellationToken cancellationToken)
    {
        // Step 1: Use a rough bounding box filter in SQL first (translatable, uses the
        // index on Latitude/Longitude) to avoid pulling the entire table into memory.
        const double approxKmPerDegree = 111.0;
        var degreeDelta = request.RadiusKm / approxKmPerDegree;

        var candidateSpots = await _unitOfWork.Repository<FishingSpot>().Query()
            .Where(s =>
                s.Latitude >= request.Latitude - degreeDelta && s.Latitude <= request.Latitude + degreeDelta &&
                s.Longitude >= request.Longitude - degreeDelta && s.Longitude <= request.Longitude + degreeDelta)
            .ToListAsync(cancellationToken);

        // Step 2: Apply the precise Haversine distance calculation client-side (in memory)
        // on the much smaller candidate set, and filter/sort by actual distance.
        var results = candidateSpots
            .Select(s => new FishingSpotDto
            {
                Id = s.Id,
                Name = s.Name,
                Description = s.Description,
                Latitude = s.Latitude,
                Longitude = s.Longitude,
                Type = s.Type,
                IsVerified = s.IsVerified,
                DistanceKm = GeoUtils.CalculateDistanceKm(request.Latitude, request.Longitude, s.Latitude, s.Longitude)
            })
            .Where(dto => dto.DistanceKm <= request.RadiusKm)
            .OrderBy(dto => dto.DistanceKm)
            .ToList();

        return Result<List<FishingSpotDto>>.Success(results);
    }
}