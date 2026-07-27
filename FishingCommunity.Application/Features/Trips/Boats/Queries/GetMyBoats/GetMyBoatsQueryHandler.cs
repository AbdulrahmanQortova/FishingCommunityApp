using FishingCommunity.Application.Common.Extensions;
using FishingCommunity.Domain.Entities.Trips;
using FishingCommunity.Domain.Interfaces;
using FishingCommunity.Shared.Wrappers;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace FishingCommunity.Application.Features.Trips.Boats.Queries.GetMyBoats;

public class GetMyBoatsQueryHandler : IRequestHandler<GetMyBoatsQuery, Result<List<BoatDto>>>
{
    private readonly IUnitOfWork _unitOfWork;

    public GetMyBoatsQueryHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<List<BoatDto>>> Handle(GetMyBoatsQuery request, CancellationToken cancellationToken)
    {
        var boats = await _unitOfWork.Repository<Boat>().Query()
            .Where(b => b.OwnerId == request.OwnerId)
            .Select(b => new BoatDto
            {
                Id = b.Id,
                Name = b.Name,
                Description = b.Description,
                RegistrationNumber = b.RegistrationNumber,
                Capacity = b.Capacity,
                MainPhotoUrl = b.MainPhotoUrl,
                Status = b.Status
            })
            .ToListAsync(cancellationToken);

        return Result<List<BoatDto>>.Success(boats);
    }
}