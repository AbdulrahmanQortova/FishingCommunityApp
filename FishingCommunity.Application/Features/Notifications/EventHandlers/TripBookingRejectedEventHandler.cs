using FishingCommunity.Application.Common.Interfaces;
using FishingCommunity.Application.Common.Models;
using FishingCommunity.Application.Features.Notifications.IntegrationEvents;
using FishingCommunity.Domain.Events.Trips;
using MediatR;

namespace FishingCommunity.Application.Features.Notifications.EventHandlers;

public class TripBookingRejectedEventHandler : INotificationHandler<DomainEventNotification<TripBookingRejectedEvent>>
{
    private readonly IEventBusPublisher _eventBusPublisher;

    public TripBookingRejectedEventHandler(IEventBusPublisher eventBusPublisher)
    {
        _eventBusPublisher = eventBusPublisher;
    }

    public async Task Handle(DomainEventNotification<TripBookingRejectedEvent> notification, CancellationToken cancellationToken)
    {
        var domainEvent = notification.DomainEvent;

        var message = new TripBookingRejectedIntegrationEvent
        {
            TripId = domainEvent.TripId,
            UserId = domainEvent.UserId
        };

        await _eventBusPublisher.PublishAsync("notification.trip.booking.rejected", message, cancellationToken);
    }
}