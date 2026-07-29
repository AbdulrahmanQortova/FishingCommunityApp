using FishingCommunity.Application.Common.Interfaces;
using FishingCommunity.Application.Common.Models;
using FishingCommunity.Domain.Enums;
using FishingCommunity.Domain.Events.Community;
using MediatR;

namespace FishingCommunity.Application.Features.Notifications.EventHandlers;

public class UserFollowedEventHandler : INotificationHandler<DomainEventNotification<UserFollowedEvent>>
{
    private readonly INotificationService _notificationService;

    public UserFollowedEventHandler(INotificationService notificationService)
    {
        _notificationService = notificationService;
    }

    public async Task Handle(DomainEventNotification<UserFollowedEvent> notification, CancellationToken cancellationToken)
    {
        var domainEvent = notification.DomainEvent;

        await _notificationService.CreateNotificationAsync(
            domainEvent.FollowedId,
            NotificationType.UserFollowed,
            "New follower",
            "You have a new follower!",
            domainEvent.FollowerId,
            cancellationToken);
    }
}