using FishingCommunity.Domain.Common;

namespace FishingCommunity.Domain.Entities.Map;

public class FavoriteLocation : BaseEntity
{
    public Guid UserId { get; private set; }
    public Guid FishingSpotId { get; private set; }
    public FishingSpot FishingSpot { get; private set; } = null!;

    private FavoriteLocation() { } // EF Core

    public FavoriteLocation(Guid userId, Guid fishingSpotId)
    {
        UserId = userId;
        FishingSpotId = fishingSpotId;
    }
}