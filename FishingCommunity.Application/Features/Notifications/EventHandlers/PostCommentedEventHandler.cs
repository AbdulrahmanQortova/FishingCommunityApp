using FishingCommunity.Application.Common.Interfaces;
using FishingCommunity.Application.Common.Models;
using FishingCommunity.Domain.Enums;
using FishingCommunity.Domain.Events.Community;
using MediatR;

namespace FishingCommunity.Application.Features.Notifications.EventHandlers;

public class PostCommentedEventHandler : INotificationHandler<DomainEventNotification<PostCommentedEvent>>
{
    private readonly INotificationService _notificationService;

    public PostCommentedEventHandler(INotificationService notificationService)
    {
        _notificationService = notificationService;
    }

    public async Task Handle(DomainEventNotification<PostCommentedEvent> notification, CancellationToken cancellationToken)
    {
        var domainEvent = notification.DomainEvent;

        // Don't notify users about their own comments on their own posts.
        if (domainEvent.CommenterId == domainEvent.PostAuthorId) return;

        await _notificationService.CreateNotificationAsync(
            domainEvent.PostAuthorId,
            NotificationType.PostCommented,
            "New comment on your post",
            "Someone commented on your post.",
            domainEvent.PostId,
            cancellationToken);
    }
}