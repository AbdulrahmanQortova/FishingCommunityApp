using FishingCommunity.Application.Common.Interfaces;
using FishingCommunity.Application.Common.Models;
using FishingCommunity.Application.Features.Notifications.IntegrationEvents;
using FishingCommunity.Domain.Events.Trips;
using MediatR;

namespace FishingCommunity.Application.Features.Notifications.EventHandlers;

public class TripBookingApprovedEventHandler : INotificationHandler<DomainEventNotification<TripBookingApprovedEvent>>
{
    private readonly IEventBusPublisher _eventBusPublisher;

    public TripBookingApprovedEventHandler(IEventBusPublisher eventBusPublisher)
    {
        _eventBusPublisher = eventBusPublisher;
    }

    public async Task Handle(DomainEventNotification<TripBookingApprovedEvent> notification, CancellationToken cancellationToken)
    {
        var domainEvent = notification.DomainEvent;

        // Instead of creating the notification directly (in-process, synchronous),
        // publish an integration event to RabbitMQ. A separate Worker Service
        // consumes it and creates the actual Notification row — this handler's
        // only job now is announcing "this happened", not deciding what to do about it.
        var message = new TripBookingApprovedIntegrationEvent
        {
            TripId = domainEvent.TripId,
            UserId = domainEvent.UserId
        };

        await _eventBusPublisher.PublishAsync("notification.trip.booking.approved", message, cancellationToken);
    }
}