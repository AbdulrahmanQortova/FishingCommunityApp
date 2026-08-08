using FishingCommunity.Application.Common.Interfaces;
using FishingCommunity.Application.Common.Models;
using FishingCommunity.Application.Features.Notifications.IntegrationEvents;
using FishingCommunity.Domain.Events.Community;
using MediatR;

namespace FishingCommunity.Application.Features.Notifications.EventHandlers;

public class PostCommentedEventHandler : INotificationHandler<DomainEventNotification<PostCommentedEvent>>
{
    private readonly IEventBusPublisher _eventBusPublisher;

    public PostCommentedEventHandler(IEventBusPublisher eventBusPublisher)
    {
        _eventBusPublisher = eventBusPublisher;
    }

    public async Task Handle(DomainEventNotification<PostCommentedEvent> notification, CancellationToken cancellationToken)
    {
        var domainEvent = notification.DomainEvent;

        if (domainEvent.CommenterId == domainEvent.PostAuthorId) return;

        var message = new PostCommentedIntegrationEvent
        {
            PostId = domainEvent.PostId,
            CommenterId = domainEvent.CommenterId,
            PostAuthorId = domainEvent.PostAuthorId
        };

        await _eventBusPublisher.PublishAsync("notification.post.commented", message, cancellationToken);
    }
}