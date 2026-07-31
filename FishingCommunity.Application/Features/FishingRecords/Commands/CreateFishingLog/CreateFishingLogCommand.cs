using FishingCommunity.Shared.Wrappers;
using MediatR;

namespace FishingCommunity.Application.Features.FishingRecords.Commands.CreateFishingLog;

public class CreateFishingLogCommand : IRequest<Result<CreateFishingLogResponse>>
{
    public Guid UserId { get; set; }
    public Guid FishSpeciesId { get; set; }
    public DateTime CaughtDate { get; set; }
    public double? WeightKg { get; set; }
    public double? LengthCm { get; set; }
    public string? LocationName { get; set; }
    public double? Latitude { get; set; }
    public double? Longitude { get; set; }
    public string? Bait { get; set; }
    public string? Notes { get; set; }
    public List<string>? PhotoUrls { get; set; }

    // If true and coordinates are provided, attempt to capture a weather snapshot.
    public bool CaptureWeather { get; set; } = true;
}