using FishingCommunity.Domain.Entities.Map;
using FishingCommunity.Domain.Exceptions;
using FishingCommunity.Domain.Interfaces;
using FishingCommunity.Shared.Wrappers;
using MediatR;

namespace FishingCommunity.Application.Features.Map.FavoriteLocations.Commands.ToggleFavoriteLocation;

public class ToggleFavoriteLocationCommandHandler : IRequestHandler<ToggleFavoriteLocationCommand, Result<bool>>
{
    private readonly IUnitOfWork _unitOfWork;

    public ToggleFavoriteLocationCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<bool>> Handle(ToggleFavoriteLocationCommand request, CancellationToken cancellationToken)
    {
        var spotExists = await _unitOfWork.Repository<FishingSpot>().AnyAsync(s => s.Id == request.FishingSpotId, cancellationToken);

        if (!spotExists)
        {
            throw new NotFoundException(nameof(FishingSpot), request.FishingSpotId);
        }

        var existing = (await _unitOfWork.Repository<FavoriteLocation>()
            .FindAsync(f => f.UserId == request.UserId && f.FishingSpotId == request.FishingSpotId, cancellationToken))
            .FirstOrDefault();

        if (existing is not null)
        {
            _unitOfWork.Repository<FavoriteLocation>().Remove(existing);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            return Result<bool>.Success(false, "Removed from favorites.");
        }

        var favorite = new FavoriteLocation(request.UserId, request.FishingSpotId);
        await _unitOfWork.Repository<FavoriteLocation>().AddAsync(favorite, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result<bool>.Success(true, "Added to favorites.");
    }
}