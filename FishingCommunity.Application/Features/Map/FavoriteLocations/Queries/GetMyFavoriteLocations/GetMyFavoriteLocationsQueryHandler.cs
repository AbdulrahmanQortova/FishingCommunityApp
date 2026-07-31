using FishingCommunity.Domain.Entities.Map;
using FishingCommunity.Domain.Interfaces;
using FishingCommunity.Shared.Wrappers;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace FishingCommunity.Application.Features.Map.FavoriteLocations.Queries.GetMyFavoriteLocations;

public class GetMyFavoriteLocationsQueryHandler : IRequestHandler<GetMyFavoriteLocationsQuery, Result<List<FavoriteLocationDto>>>
{
    private readonly IUnitOfWork _unitOfWork;

    public GetMyFavoriteLocationsQueryHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<List<FavoriteLocationDto>>> Handle(GetMyFavoriteLocationsQuery request, CancellationToken cancellationToken)
    {
        var favorites = await _unitOfWork.Repository<FavoriteLocation>().Query()
            .Where(f => f.UserId == request.UserId)
            .Select(f => new FavoriteLocationDto
            {
                FishingSpotId = f.FishingSpotId,
                Name = f.FishingSpot.Name,
                Latitude = f.FishingSpot.Latitude,
                Longitude = f.FishingSpot.Longitude,
                Type = f.FishingSpot.Type
            })
            .ToListAsync(cancellationToken);

        return Result<List<FavoriteLocationDto>>.Success(favorites);
    }
}