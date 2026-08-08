using FishingCommunity.Application.Common.Interfaces;
using FishingCommunity.Application.Common.Models;
using FishingCommunity.Application.Features.Notifications.IntegrationEvents;
using FishingCommunity.Domain.Entities.Trips;
using FishingCommunity.Domain.Events.Trips;
using FishingCommunity.Domain.Interfaces;
using MediatR;

namespace FishingCommunity.Application.Features.Notifications.EventHandlers;

public class TripBookingRequestedEventHandler : INotificationHandler<DomainEventNotification<TripBookingRequestedEvent>>
{
    private readonly IEventBusPublisher _eventBusPublisher;
    private readonly IUnitOfWork _unitOfWork;

    public TripBookingRequestedEventHandler(IEventBusPublisher eventBusPublisher, IUnitOfWork unitOfWork)
    {
        _eventBusPublisher = eventBusPublisher;
        _unitOfWork = unitOfWork;
    }

    public async Task Handle(DomainEventNotification<TripBookingRequestedEvent> notification, CancellationToken cancellationToken)
    {
        var domainEvent = notification.DomainEvent;

        // Still need to resolve the organizer + trip title here (in the API), since
        // the Worker Service doesn't query the database itself in this simple setup —
        // see the note on TripCancelledEventHandler for the reasoning.
        var trip = await _unitOfWork.Repository<Trip>().GetByIdAsync(domainEvent.TripId, cancellationToken);
        if (trip is null) return;

        var message = new TripBookingRequestedIntegrationEvent
        {
            TripId = domainEvent.TripId,
            TripTitle = trip.Title,
            OrganizerId = trip.OrganizerId
        };

        await _eventBusPublisher.PublishAsync("notification.trip.booking.requested", message, cancellationToken);
    }
}