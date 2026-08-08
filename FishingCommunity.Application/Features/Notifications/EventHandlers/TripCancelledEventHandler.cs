using FishingCommunity.Application.Common.Interfaces;
using FishingCommunity.Application.Common.Models;
using FishingCommunity.Application.Features.Notifications.IntegrationEvents;
using FishingCommunity.Domain.Entities.Trips;
using FishingCommunity.Domain.Events.Trips;
using FishingCommunity.Domain.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace FishingCommunity.Application.Features.Notifications.EventHandlers;

public class TripCancelledEventHandler : INotificationHandler<DomainEventNotification<TripCancelledEvent>>
{
    private readonly IEventBusPublisher _eventBusPublisher;
    private readonly IUnitOfWork _unitOfWork;

    public TripCancelledEventHandler(IEventBusPublisher eventBusPublisher, IUnitOfWork unitOfWork)
    {
        _eventBusPublisher = eventBusPublisher;
        _unitOfWork = unitOfWork;
    }

    public async Task Handle(DomainEventNotification<TripCancelledEvent> notification, CancellationToken cancellationToken)
    {
        var domainEvent = notification.DomainEvent;

        // We still need this query HERE, in the API — the Worker Service won't have
        // its own DbContext connection to the same data in this simple setup, so it's
        // simpler to resolve "who's affected" now and send their IDs in the message,
        // rather than have the Consumer re-query the database itself.
        var affectedUserIds = await _unitOfWork.Repository<TripBooking>().Query()
            .Where(b => b.TripId == domainEvent.TripId)
            .Select(b => b.UserId)
            .Distinct()
            .ToListAsync(cancellationToken);

        var message = new TripCancelledIntegrationEvent
        {
            TripId = domainEvent.TripId,
            Reason = domainEvent.Reason,
            AffectedUserIds = affectedUserIds
        };

        await _eventBusPublisher.PublishAsync("notification.trip.cancelled", message, cancellationToken);
    }
}