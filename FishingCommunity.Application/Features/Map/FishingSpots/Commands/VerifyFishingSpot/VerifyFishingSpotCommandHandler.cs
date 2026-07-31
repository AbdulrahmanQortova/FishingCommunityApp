using FishingCommunity.Domain.Entities.Map;
using FishingCommunity.Domain.Exceptions;
using FishingCommunity.Domain.Interfaces;
using FishingCommunity.Shared.Wrappers;
using MediatR;

namespace FishingCommunity.Application.Features.Map.FishingSpots.Commands.VerifyFishingSpot;

public class VerifyFishingSpotCommandHandler : IRequestHandler<VerifyFishingSpotCommand, Result>
{
    private readonly IUnitOfWork _unitOfWork;

    public VerifyFishingSpotCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> Handle(VerifyFishingSpotCommand request, CancellationToken cancellationToken)
    {
        var spot = await _unitOfWork.Repository<FishingSpot>().GetByIdAsync(request.FishingSpotId, cancellationToken);

        if (spot is null)
        {
            throw new NotFoundException(nameof(FishingSpot), request.FishingSpotId);
        }

        spot.Verify();

        _unitOfWork.Repository<FishingSpot>().Update(spot);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success("Fishing spot verified successfully.");
    }
}