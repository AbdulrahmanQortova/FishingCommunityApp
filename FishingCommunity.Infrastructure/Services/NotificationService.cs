using FishingCommunity.Application.Common.Interfaces;
using FishingCommunity.Domain.Entities.Notifications;
using FishingCommunity.Domain.Enums;
using FishingCommunity.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace FishingCommunity.Infrastructure.Services;

public class NotificationService : INotificationService
{
    private readonly IServiceScopeFactory _scopeFactory;

    public NotificationService(IServiceScopeFactory scopeFactory)
    {
        _scopeFactory = scopeFactory;
    }

    public async Task CreateNotificationAsync(
        Guid recipientUserId,
        NotificationType type,
        string title,
        string message,
        Guid? relatedEntityId = null,
        CancellationToken cancellationToken = default)
    {
        // Uses a fresh DI scope (and therefore a fresh DbContext instance) instead of
        // reusing the ambient one — this runs from inside SavedChangesAsync on the
        // original context, so sharing it here risks re-entrancy on the same DbContext.
        using var scope = _scopeFactory.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        var notification = new Notification(recipientUserId, type, title, message, relatedEntityId);

        dbContext.Notifications.Add(notification);
        await dbContext.SaveChangesAsync(cancellationToken);
    }
}