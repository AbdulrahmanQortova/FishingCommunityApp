using FishingCommunity.Domain.Enums;

namespace FishingCommunity.Application.Common.Interfaces;

public interface INotificationService
{
    Task CreateNotificationAsync(
        Guid recipientUserId,
        NotificationType type,
        string title,
        string message,
        Guid? relatedEntityId = null,
        CancellationToken cancellationToken = default);
}