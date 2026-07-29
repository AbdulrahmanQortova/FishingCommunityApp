using FishingCommunity.Application.Common.Interfaces;
using FishingCommunity.Application.Common.Models;
using FishingCommunity.Domain.Entities.Trips;
using FishingCommunity.Domain.Enums;
using FishingCommunity.Domain.Events.Trips;
using FishingCommunity.Domain.Interfaces;
using MediatR;

namespace FishingCommunity.Application.Features.Notifications.EventHandlers;

public class TripBookingRequestedEventHandler : INotificationHandler<DomainEventNotification<TripBookingRequestedEvent>>
{
    private readonly INotificationService _notificationService;
    private readonly IUnitOfWork _unitOfWork;

    public TripBookingRequestedEventHandler(INotificationService notificationService, IUnitOfWork unitOfWork)
    {
        _notificationService = notificationService;
        _unitOfWork = unitOfWork;
    }

    public async Task Handle(DomainEventNotification<TripBookingRequestedEvent> notification, CancellationToken cancellationToken)
    {
        var domainEvent = notification.DomainEvent;

        var trip = await _unitOfWork.Repository<Trip>().GetByIdAsync(domainEvent.TripId, cancellationToken);
        if (trip is null) return; // Defensive — shouldn't happen, but don't crash notification pipeline over it.

        // Notify the trip organizer that someone requested a booking.
        await _notificationService.CreateNotificationAsync(
            trip.OrganizerId,
            NotificationType.TripBookingRequested,
            "New booking request",
            $"Someone requested to book a seat on your trip \"{trip.Title}\".",
            domainEvent.TripId,
            cancellationToken);
    }
}