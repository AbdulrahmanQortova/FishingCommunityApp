using FishingCommunity.Domain.Entities.Map;
using FishingCommunity.Domain.Interfaces;
using FishingCommunity.Shared.Wrappers;
using MediatR;

namespace FishingCommunity.Application.Features.Map.FishingSpots.Commands.CreateFishingSpot;

public class CreateFishingSpotCommandHandler : IRequestHandler<CreateFishingSpotCommand, Result<Guid>>
{
    private readonly IUnitOfWork _unitOfWork;

    public CreateFishingSpotCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<Guid>> Handle(CreateFishingSpotCommand request, CancellationToken cancellationToken)
    {
        var spot = new FishingSpot(request.CreatedByUserId, request.Name, request.Latitude, request.Longitude, request.Type, request.Description);

        await _unitOfWork.Repository<FishingSpot>().AddAsync(spot, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result<Guid>.Success(spot.Id, "Fishing spot added successfully. It will be marked as verified after review.");
    }
}