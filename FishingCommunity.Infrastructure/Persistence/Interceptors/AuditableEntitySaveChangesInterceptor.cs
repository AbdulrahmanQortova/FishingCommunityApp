using FishingCommunity.Application.Common.Interfaces;
using FishingCommunity.Domain.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace FishingCommunity.Infrastructure.Persistence.Interceptors;

public class AuditableEntitySaveChangesInterceptor : SaveChangesInterceptor
{
    private readonly ICurrentUserService _currentUserService;
    private readonly IDateTimeProvider _dateTimeProvider;

    public AuditableEntitySaveChangesInterceptor(
        ICurrentUserService currentUserService,
        IDateTimeProvider dateTimeProvider)
    {
        _currentUserService = currentUserService;
        _dateTimeProvider = dateTimeProvider;
    }

    public override InterceptionResult<int> SavingChanges(DbContextEventData eventData, InterceptionResult<int> result)
    {
        UpdateAuditableEntities(eventData.Context);
        return base.SavingChanges(eventData, result);
    }

    public override ValueTask<InterceptionResult<int>> SavingChangesAsync(
        DbContextEventData eventData,
        InterceptionResult<int> result,
        CancellationToken cancellationToken = default)
    {
        UpdateAuditableEntities(eventData.Context);
        return base.SavingChangesAsync(eventData, result, cancellationToken);
    }

    private void UpdateAuditableEntities(DbContext? context)
    {
        if (context is null) return;

        var utcNow = _dateTimeProvider.UtcNow;
        var currentUserId = _currentUserService.UserId;

        // --- Handle BaseAuditableEntity (regular domain entities: Posts, Trips, Orders, etc.) ---
        foreach (var entry in context.ChangeTracker.Entries<BaseAuditableEntity>())
        {
            switch (entry.State)
            {
                case EntityState.Added:
                    entry.Entity.CreatedBy = currentUserId;
                    entry.Entity.CreatedDate = utcNow;
                    break;

                case EntityState.Modified:
                    entry.Entity.UpdatedBy = currentUserId;
                    entry.Entity.UpdatedDate = utcNow;
                    break;

                case EntityState.Deleted:
                    // Convert hard delete into soft delete.
                    SoftDeleteEntry(entry, currentUserId, utcNow);
                    break;
            }
        }

        // --- Handle ApplicationUser separately (it inherits IdentityUser<Guid>, not BaseAuditableEntity) ---
        foreach (var entry in context.ChangeTracker.Entries<Domain.Entities.Identity.ApplicationUser>())
        {
            switch (entry.State)
            {
                case EntityState.Added:
                    entry.Property(nameof(Domain.Entities.Identity.ApplicationUser.CreatedDate)).CurrentValue = utcNow;
                    entry.Property(nameof(Domain.Entities.Identity.ApplicationUser.CreatedBy)).CurrentValue = currentUserId;
                    break;

                case EntityState.Modified:
                    entry.Property(nameof(Domain.Entities.Identity.ApplicationUser.UpdatedDate)).CurrentValue = utcNow;
                    entry.Property(nameof(Domain.Entities.Identity.ApplicationUser.UpdatedBy)).CurrentValue = currentUserId;
                    break;

                case EntityState.Deleted:
                    entry.State = EntityState.Modified;
                    entry.Property(nameof(Domain.Entities.Identity.ApplicationUser.IsDeleted)).CurrentValue = true;
                    entry.Property(nameof(Domain.Entities.Identity.ApplicationUser.DeletedDate)).CurrentValue = utcNow;
                    entry.Property(nameof(Domain.Entities.Identity.ApplicationUser.DeletedBy)).CurrentValue = currentUserId;
                    break;
            }
        }
    }

    private static void SoftDeleteEntry(EntityEntry<BaseAuditableEntity> entry, Guid? currentUserId, DateTime utcNow)
    {
        entry.State = EntityState.Modified;
        entry.Entity.IsDeleted = true;
        entry.Entity.DeletedDate = utcNow;
        entry.Entity.DeletedBy = currentUserId;
    }
}