namespace FishingCommunity.Domain.Common;

/// <summary>
/// Marker interface to indicate an entity is an Aggregate Root in DDD terms.
/// Repositories should only be created for Aggregate Roots.
/// </summary>
public interface IAggregateRoot
{
}