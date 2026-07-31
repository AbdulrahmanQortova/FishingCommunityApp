using FishingCommunity.Application.Common.Interfaces;
using FishingCommunity.Domain.Entities.FishingRecords;
using FishingCommunity.Domain.Exceptions;
using FishingCommunity.Domain.Interfaces;
using FishingCommunity.Shared.Wrappers;
using MediatR;
using FishSpeciesEntity = FishingCommunity.Domain.Entities.FishingRecords.FishSpecies;

namespace FishingCommunity.Application.Features.FishingRecords.Commands.CreateFishingLog;

public class CreateFishingLogCommandHandler : IRequestHandler<CreateFishingLogCommand, Result<CreateFishingLogResponse>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IWeatherService _weatherService;

    public CreateFishingLogCommandHandler(IUnitOfWork unitOfWork, IWeatherService weatherService)
    {
        _unitOfWork = unitOfWork;
        _weatherService = weatherService;
    }

    public async Task<Result<CreateFishingLogResponse>> Handle(CreateFishingLogCommand request, CancellationToken cancellationToken)
    {
        var fishSpecies = await _unitOfWork.Repository<FishSpeciesEntity>().GetByIdAsync(request.FishSpeciesId, cancellationToken);

        if (fishSpecies is null)
        {
            throw new NotFoundException(nameof(FishSpeciesEntity), request.FishSpeciesId);
        }

        var log = new FishingLog(
            request.UserId,
            request.FishSpeciesId,
            request.CaughtDate,
            request.WeightKg,
            request.LengthCm,
            request.LocationName,
            request.Latitude,
            request.Longitude,
            request.Bait,
            request.Notes);

        if (request.PhotoUrls is not null)
        {
            foreach (var url in request.PhotoUrls)
            {
                log.AddPhoto(url);
            }
        }

        if (request.CaptureWeather && request.Latitude.HasValue && request.Longitude.HasValue)
        {
            var weather = await _weatherService.GetCurrentWeatherAsync(request.Latitude.Value, request.Longitude.Value, cancellationToken);

            if (weather is not null)
            {
                log.AttachWeatherSnapshot(weather.Temperature, weather.Description);
            }
        }

        await _unitOfWork.Repository<FishingLog>().AddAsync(log, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        var response = new CreateFishingLogResponse
        {
            FishingLogId = log.Id,
            FishSpeciesName = fishSpecies.Name,
            CaughtDate = log.CaughtDate
        };

        return Result<CreateFishingLogResponse>.Success(response, "Fishing log created successfully.");
    }
}