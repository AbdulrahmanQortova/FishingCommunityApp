using FishingCommunity.Application.Common.Interfaces;
using FishingCommunity.Application.Common.Models;
using FishingCommunity.Application.Features.Notifications.IntegrationEvents;
using FishingCommunity.Domain.Events.Community;
using MediatR;

namespace FishingCommunity.Application.Features.Notifications.EventHandlers;

public class PostLikedEventHandler : INotificationHandler<DomainEventNotification<PostLikedEvent>>
{
    private readonly IEventBusPublisher _eventBusPublisher;

    public PostLikedEventHandler(IEventBusPublisher eventBusPublisher)
    {
        _eventBusPublisher = eventBusPublisher;
    }

    public async Task Handle(DomainEventNotification<PostLikedEvent> notification, CancellationToken cancellationToken)
    {
        var domainEvent = notification.DomainEvent;

        if (domainEvent.LikedByUserId == domainEvent.PostAuthorId) return;

        var message = new PostLikedIntegrationEvent
        {
            PostId = domainEvent.PostId,
            LikedByUserId = domainEvent.LikedByUserId,
            PostAuthorId = domainEvent.PostAuthorId
        };

        await _eventBusPublisher.PublishAsync("notification.post.liked", message, cancellationToken);
    }
}