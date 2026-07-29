using FishingCommunity.Domain.Common;
using FishingCommunity.Domain.Enums;

namespace FishingCommunity.Domain.Entities.Notifications;

public class Notification : BaseAuditableEntity
{
    public Guid RecipientUserId { get; private set; }
    public NotificationType Type { get; private set; }
    public string Title { get; private set; } = string.Empty;
    public string Message { get; private set; } = string.Empty;
    public Guid? RelatedEntityId { get; private set; }

    public bool IsRead { get; private set; }
    public DateTime? ReadDate { get; private set; }

    private Notification() { } // EF Core

    public Notification(Guid recipientUserId, NotificationType type, string title, string message, Guid? relatedEntityId = null)
    {
        RecipientUserId = recipientUserId;
        Type = type;
        Title = title;
        Message = message;
        RelatedEntityId = relatedEntityId;
    }

    public void MarkAsRead()
    {
        if (IsRead) return;

        IsRead = true;
        ReadDate = DateTime.UtcNow;
    }
}