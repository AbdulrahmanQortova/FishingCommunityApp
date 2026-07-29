using FishingCommunity.Application.Common.Interfaces;
using FishingCommunity.Application.Common.Models;
using FishingCommunity.Domain.Enums;
using FishingCommunity.Domain.Events.Community;
using MediatR;

namespace FishingCommunity.Application.Features.Notifications.EventHandlers;

public class PostLikedEventHandler : INotificationHandler<DomainEventNotification<PostLikedEvent>>
{
    private readonly INotificationService _notificationService;

    public PostLikedEventHandler(INotificationService notificationService)
    {
        _notificationService = notificationService;
    }

    public async Task Handle(DomainEventNotification<PostLikedEvent> notification, CancellationToken cancellationToken)
    {
        var domainEvent = notification.DomainEvent;

        if (domainEvent.LikedByUserId == domainEvent.PostAuthorId) return;

        await _notificationService.CreateNotificationAsync(
            domainEvent.PostAuthorId,
            NotificationType.PostLiked,
            "New like",
            "Someone liked your post.",
            domainEvent.PostId,
            cancellationToken);
    }
}