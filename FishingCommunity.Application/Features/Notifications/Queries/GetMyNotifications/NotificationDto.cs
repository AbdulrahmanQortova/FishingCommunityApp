using FishingCommunity.Domain.Enums;

namespace FishingCommunity.Application.Features.Notifications.Queries.GetMyNotifications;

public class NotificationDto
{
    public Guid Id { get; set; }
    public NotificationType Type { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public Guid? RelatedEntityId { get; set; }
    public bool IsRead { get; set; }
    public DateTime CreatedDate { get; set; }
}