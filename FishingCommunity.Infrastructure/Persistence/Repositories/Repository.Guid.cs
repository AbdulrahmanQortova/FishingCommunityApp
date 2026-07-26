using FishingCommunity.Domain.Common;
using FishingCommunity.Domain.Interfaces;

namespace FishingCommunity.Infrastructure.Persistence.Repositories;

public class Repository<TEntity> : Repository<TEntity, Guid>, IRepository<TEntity>
    where TEntity : BaseEntity
{
    public Repository(ApplicationDbContext context) : base(context)
    {
    }
}