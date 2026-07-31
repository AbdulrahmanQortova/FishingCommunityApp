using FishingCommunity.Domain.Interfaces;
using FishingCommunity.Shared.Wrappers;
using MediatR;

namespace FishingCommunity.Application.Features.FishingRecords.FishSpecies.Commands.CreateFishSpecies;

public class CreateFishSpeciesCommandHandler : IRequestHandler<CreateFishSpeciesCommand, Result<Guid>>
{
    private readonly IUnitOfWork _unitOfWork;

    public CreateFishSpeciesCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<Guid>> Handle(CreateFishSpeciesCommand request, CancellationToken cancellationToken)
    {
        var alreadyExists = await _unitOfWork.Repository<Domain.Entities.FishingRecords.FishSpecies>()
            .AnyAsync(f => f.Name == request.Name, cancellationToken);

        if (alreadyExists)
        {
            return Result<Guid>.Failure("This fish species already exists.");
        }

        var species = new Domain.Entities.FishingRecords.FishSpecies(request.Name, request.ScientificName, request.IconUrl);

        await _unitOfWork.Repository<Domain.Entities.FishingRecords.FishSpecies>().AddAsync(species, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result<Guid>.Success(species.Id, "Fish species added successfully.");
    }
}