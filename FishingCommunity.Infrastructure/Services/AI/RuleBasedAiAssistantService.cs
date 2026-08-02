using FishingCommunity.Application.Common.Interfaces;

namespace FishingCommunity.Infrastructure.Services.AI;

/// <summary>
/// Free, rule-based implementation of IAiAssistantService — no external API calls,
/// no cost. Uses heuristic rules based on weather conditions and species knowledge
/// instead of a real language model. Swap this registration for an OpenAI-backed
/// implementation later without touching any Application or API layer code.
/// </summary>
public class RuleBasedAiAssistantService : IAiAssistantService
{
    // A small knowledge base of common species-to-bait/rod mappings.
    // Could be moved to a database table (Admin-managed) later if it needs to grow.
    private static readonly Dictionary<string, (List<string> Rods, List<string> Bait)> EquipmentKnowledgeBase = new(StringComparer.OrdinalIgnoreCase)
    {
        ["Tuna"] = (
            Rods: new List<string> { "Heavy-action trolling rod", "80-130 lb class rod" },
            Bait: new List<string> { "Live sardines", "Cut squid", "Trolling lures" }),
        ["Snapper"] = (
            Rods: new List<string> { "Medium-heavy spinning rod", "7ft rod with fast action" },
            Bait: new List<string> { "Shrimp", "Cut bait", "Squid strips" }),
        ["Bass"] = (
            Rods: new List<string> { "Medium spinning or baitcasting rod", "6-7ft rod" },
            Bait: new List<string> { "Soft plastic worms", "Crankbaits", "Live minnows" }),
        ["Catfish"] = (
            Rods: new List<string> { "Heavy-action rod, 7-8ft", "Sturdy baitcasting reel" },
            Bait: new List<string> { "Chicken liver", "Stink bait", "Nightcrawlers" }),
        ["Grouper"] = (
            Rods: new List<string> { "Heavy bottom-fishing rod", "50-80 lb class rod" },
            Bait: new List<string> { "Live pinfish", "Squid", "Cut bait" }),
    };

    private static readonly (List<string> Rods, List<string> Bait) DefaultEquipment = (
        Rods: new List<string> { "All-purpose medium-action spinning rod" },
        Bait: new List<string> { "Live shrimp", "Artificial lures" });

    public Task<FishingRecommendationResult> GetFishingRecommendationAsync(FishingRecommendationRequest request, CancellationToken cancellationToken = default)
    {
        var result = new FishingRecommendationResult();

        // Weather-based heuristics — reuses the same general logic principles as
        // FishingSuitabilityCalculator, but phrased as advice rather than a rating.
        if (request.WindSpeed is > 10)
        {
            result.Recommendation = "Wind conditions are quite strong today. Consider fishing in a sheltered area or postponing your trip if you're using a small boat.";
        }
        else if (request.WeatherTemperature is < 10)
        {
            result.Recommendation = "Cold water temperatures often slow fish activity. Try slower presentations and focus on deeper, warmer pockets of water.";
        }
        else if (request.WeatherTemperature is > 30)
        {
            result.Recommendation = "Warm conditions — fish are likely more active early morning or late evening when temperatures are cooler.";
        }
        else
        {
            result.Recommendation = "Conditions look generally favorable for fishing today. Good luck out there!";
        }

        result.BestTimeOfDay = request.WeatherTemperature is > 28 ? "Early morning or late evening" : "Mid-morning to early afternoon";

        result.SuggestedSpecies = string.IsNullOrWhiteSpace(request.PreferredFishSpecies)
            ? new List<string> { "Snapper", "Bass", "Grouper" } // Generic common suggestions
            : new List<string> { request.PreferredFishSpecies };

        return Task.FromResult(result);
    }

    public Task<EquipmentRecommendationResult> GetEquipmentRecommendationAsync(EquipmentRecommendationRequest request, CancellationToken cancellationToken = default)
    {
        var (rods, bait) = EquipmentKnowledgeBase.TryGetValue(request.TargetSpecies, out var knownEquipment)
            ? knownEquipment
            : DefaultEquipment;

        var tips = request.ExperienceLevel switch
        {
            "Beginner" => "Start with simple, forgiving gear and practice your casting technique before targeting specific species.",
            "Intermediate" => "Experiment with different retrieval speeds and depths to see what triggers bites in current conditions.",
            "Advanced" => "Consider fine-tuning your leader material and hook size based on water clarity and pressure from other anglers.",
            _ => "Match your gear to the target species and local conditions for the best results."
        };

        var result = new EquipmentRecommendationResult
        {
            RecommendedRods = rods,
            RecommendedBait = bait,
            GeneralTips = tips
        };

        return Task.FromResult(result);
    }
}