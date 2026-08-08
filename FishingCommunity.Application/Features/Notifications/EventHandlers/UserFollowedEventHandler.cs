using FishingCommunity.Application.Common.Interfaces;
using FishingCommunity.Application.Common.Models;
using FishingCommunity.Application.Features.Notifications.IntegrationEvents;
using FishingCommunity.Domain.Events.Community;
using MediatR;

namespace FishingCommunity.Application.Features.Notifications.EventHandlers;

public class UserFollowedEventHandler : INotificationHandler<DomainEventNotification<UserFollowedEvent>>
{
    private readonly IEventBusPublisher _eventBusPublisher;

    public UserFollowedEventHandler(IEventBusPublisher eventBusPublisher)
    {
        _eventBusPublisher = eventBusPublisher;
    }

    public async Task Handle(DomainEventNotification<UserFollowedEvent> notification, CancellationToken cancellationToken)
    {
        var domainEvent = notification.DomainEvent;

        var message = new UserFollowedIntegrationEvent
        {
            FollowerId = domainEvent.FollowerId,
            FollowedId = domainEvent.FollowedId
        };

        await _eventBusPublisher.PublishAsync("notification.user.followed", message, cancellationToken);
    }
}