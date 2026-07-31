using FishingCommunity.Shared.Wrappers;
using MediatR;

namespace FishingCommunity.Application.Features.FishingRecords.Commands.UpdateFishingLog;

public class UpdateFishingLogCommand : IRequest<Result>
{
    public Guid FishingLogId { get; set; }
    public Guid RequestingUserId { get; set; }
    public double? WeightKg { get; set; }
    public double? LengthCm { get; set; }
    public string? LocationName { get; set; }
    public double? Latitude { get; set; }
    public double? Longitude { get; set; }
    public string? Bait { get; set; }
    public string? Notes { get; set; }
}