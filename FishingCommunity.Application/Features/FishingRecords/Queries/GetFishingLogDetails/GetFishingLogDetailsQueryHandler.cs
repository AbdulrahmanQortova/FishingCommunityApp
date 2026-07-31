using FishingCommunity.Domain.Entities.FishingRecords;
using FishingCommunity.Domain.Interfaces;
using FishingCommunity.Shared.Wrappers;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace FishingCommunity.Application.Features.FishingRecords.Queries.GetFishingLogDetails;

public class GetFishingLogDetailsQueryHandler : IRequestHandler<GetFishingLogDetailsQuery, Result<FishingLogDetailsDto>>
{
    private readonly IUnitOfWork _unitOfWork;

    public GetFishingLogDetailsQueryHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<FishingLogDetailsDto>> Handle(GetFishingLogDetailsQuery request, CancellationToken cancellationToken)
    {
        var log = await _unitOfWork.Repository<FishingLog>().Query()
            .Where(l => l.Id == request.FishingLogId)
            .Include(l => l.FishSpecies)
            .FirstOrDefaultAsync(cancellationToken);

        if (log is null)
        {
            return Result<FishingLogDetailsDto>.Failure("Fishing log not found.");
        }

        var dto = new FishingLogDetailsDto
        {
            Id = log.Id,
            UserId = log.UserId,
            FishSpeciesName = log.FishSpecies.Name,
            WeightKg = log.WeightKg,
            LengthCm = log.LengthCm,
            LocationName = log.LocationName,
            Latitude = log.Latitude,
            Longitude = log.Longitude,
            Bait = log.Bait,
            Notes = log.Notes,
            CaughtDate = log.CaughtDate,
            WeatherTemperature = log.WeatherTemperature,
            WeatherDescription = log.WeatherDescription,
            PhotoUrls = log.PhotoUrls.ToList()
        };

        return Result<FishingLogDetailsDto>.Success(dto);
    }
}