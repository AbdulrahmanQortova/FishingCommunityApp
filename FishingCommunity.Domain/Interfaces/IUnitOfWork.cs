using FishingCommunity.Domain.Common;

namespace FishingCommunity.Domain.Interfaces;

public interface IUnitOfWork : IDisposable
{
    /// <summary>
    /// Gets a repository for entities using the default Guid identifier.
    /// Covers the vast majority of entities in the system.
    /// </summary>
    IRepository<TEntity> Repository<TEntity>() where TEntity : BaseEntity;

    /// <summary>
    /// Gets a repository for entities using a custom identifier type.
    /// Use only for the rare entities that don't use Guid as their Id.
    /// </summary>
    IRepository<TEntity, TId> Repository<TEntity, TId>() where TEntity : BaseEntity<TId>;

    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);

    Task BeginTransactionAsync(CancellationToken cancellationToken = default);
    Task CommitTransactionAsync(CancellationToken cancellationToken = default);
    Task RollbackTransactionAsync(CancellationToken cancellationToken = default);
}