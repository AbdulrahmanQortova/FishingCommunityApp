namespace FishingCommunity.Application.Common.Models;

public class FeatureFlags
{
    public const string SectionName = "FeatureFlags";

    public bool RequireEmailVerification { get; set; } = true;
}