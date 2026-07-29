using FishingCommunity.Application.Common.Interfaces;
using FishingCommunity.Application.Common.Models;
using FishingCommunity.Domain.Enums;
using FishingCommunity.Domain.Events.Trips;
using MediatR;

namespace FishingCommunity.Application.Features.Notifications.EventHandlers;

public class TripBookingRejectedEventHandler : INotificationHandler<DomainEventNotification<TripBookingRejectedEvent>>
{
    private readonly INotificationService _notificationService;

    public TripBookingRejectedEventHandler(INotificationService notificationService)
    {
        _notificationService = notificationService;
    }

    public async Task Handle(DomainEventNotification<TripBookingRejectedEvent> notification, CancellationToken cancellationToken)
    {
        var domainEvent = notification.DomainEvent;

        await _notificationService.CreateNotificationAsync(
            domainEvent.UserId,
            NotificationType.TripBookingRejected,
            "Booking not approved",
            "Unfortunately, your booking request was not approved this time.",
            domainEvent.TripId,
            cancellationToken);
    }
}