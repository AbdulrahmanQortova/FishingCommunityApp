using FishingCommunity.Shared.Wrappers;
using MediatR;

namespace FishingCommunity.Application.Features.FishingRecords.FishSpecies.Commands.CreateFishSpecies;

public class CreateFishSpeciesCommand : IRequest<Result<Guid>>
{
    public string Name { get; set; } = string.Empty;
    public string? ScientificName { get; set; }
    public string? IconUrl { get; set; }
}