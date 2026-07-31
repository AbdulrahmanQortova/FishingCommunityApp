using FishingCommunity.Domain.Interfaces;
using FishingCommunity.Shared.Wrappers;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace FishingCommunity.Application.Features.FishingRecords.FishSpecies.Queries.GetAllFishSpecies;

public class GetAllFishSpeciesQueryHandler : IRequestHandler<GetAllFishSpeciesQuery, Result<List<FishSpeciesDto>>>
{
    private readonly IUnitOfWork _unitOfWork;

    public GetAllFishSpeciesQueryHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<List<FishSpeciesDto>>> Handle(GetAllFishSpeciesQuery request, CancellationToken cancellationToken)
    {
        var species = await _unitOfWork.Repository<Domain.Entities.FishingRecords.FishSpecies>().Query()
            .OrderBy(f => f.Name)
            .Select(f => new FishSpeciesDto
            {
                Id = f.Id,
                Name = f.Name,
                ScientificName = f.ScientificName,
                IconUrl = f.IconUrl
            })
            .ToListAsync(cancellationToken);

        return Result<List<FishSpeciesDto>>.Success(species);
    }
}