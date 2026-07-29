using FishingCommunity.Application.Common.Interfaces;
using FishingCommunity.Application.Common.Models;
using FishingCommunity.Domain.Common;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace FishingCommunity.Infrastructure.Persistence.Interceptors;

public class AuditableEntitySaveChangesInterceptor : SaveChangesInterceptor
{
    private readonly ICurrentUserService _currentUserService;
    private readonly IDateTimeProvider _dateTimeProvider;
    private readonly IPublisher _publisher;

    public AuditableEntitySaveChangesInterceptor(
        ICurrentUserService currentUserService,
        IDateTimeProvider dateTimeProvider,
        IPublisher publisher)
    {
        _currentUserService = currentUserService;
        _dateTimeProvider = dateTimeProvider;
        _publisher = publisher;
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

    public override async ValueTask<int> SavedChangesAsync(
        SaveChangesCompletedEventData eventData,
        int result,
        CancellationToken cancellationToken = default)
    {
        // Dispatch domain events only AFTER the changes are successfully persisted —
        // this guarantees a handler reacting to e.g. TripCreatedEvent can safely assume
        // the Trip row actually exists in the database by the time it runs.
        if (eventData.Context is not null)
        {
            await DispatchDomainEventsAsync(eventData.Context, cancellationToken);
        }

        return await base.SavedChangesAsync(eventData, result, cancellationToken);
    }

    private async Task DispatchDomainEventsAsync(DbContext context, CancellationToken cancellationToken)
    {
        var entitiesWithEvents = context.ChangeTracker.Entries<BaseEntity>()
            .Select(e => e.Entity)
            .Where(e => e.DomainEvents.Any())
            .ToList();

        var domainEvents = entitiesWithEvents
            .SelectMany(e => e.DomainEvents)
            .ToList();

        entitiesWithEvents.ForEach(e => e.ClearDomainEvents());

        foreach (var domainEvent in domainEvents)
        {
            var notificationType = typeof(DomainEventNotification<>).MakeGenericType(domainEvent.GetType());
            var notification = Activator.CreateInstance(notificationType, domainEvent)!;

            await _publisher.Publish(notification, cancellationToken);
        }
    }

    private void UpdateAuditableEntities(DbContext? context)
    {
        if (context is null) return;

        var utcNow = _dateTimeProvider.UtcNow;
        var currentUserId = _currentUserService.UserId;

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
                    SoftDeleteEntry(entry, currentUserId, utcNow);
                    break;
            }
        }

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