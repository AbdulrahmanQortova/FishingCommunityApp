namespace FishingCommunity.Application.Features.FishingRecords.Commands.CreateFishingLog;

public class CreateFishingLogResponse
{
    public Guid FishingLogId { get; set; }
    public string FishSpeciesName { get; set; } = string.Empty;
    public DateTime CaughtDate { get; set; }
}