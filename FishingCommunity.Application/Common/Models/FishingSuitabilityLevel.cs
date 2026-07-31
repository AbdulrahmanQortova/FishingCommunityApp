namespace FishingCommunity.Application.Common.Models;

public enum FishingSuitabilityLevel
{
    Poor = 1,
    Fair = 2,
    Good = 3,
    Excellent = 4
}

public class FishingSuitability
{
    public FishingSuitabilityLevel Level { get; set; }
    public string Reason { get; set; } = string.Empty;
}