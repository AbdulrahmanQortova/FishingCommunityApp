using FishingCommunity.Shared.Wrappers;
using MediatR;

namespace FishingCommunity.Application.Features.FishingRecords.FishSpecies.Queries.GetAllFishSpecies;

public class GetAllFishSpeciesQuery : IRequest<Result<List<FishSpeciesDto>>>
{
}