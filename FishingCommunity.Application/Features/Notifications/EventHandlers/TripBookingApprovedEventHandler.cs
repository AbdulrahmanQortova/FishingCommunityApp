using FishingCommunity.Application.Common.Interfaces;
using FishingCommunity.Application.Common.Models;
using FishingCommunity.Domain.Enums;
using FishingCommunity.Domain.Events.Trips;
using MediatR;

namespace FishingCommunity.Application.Features.Notifications.EventHandlers;

public class TripBookingApprovedEventHandler : INotificationHandler<DomainEventNotification<TripBookingApprovedEvent>>
{
    private readonly INotificationService _notificationService;

    public TripBookingApprovedEventHandler(INotificationService notificationService)
    {
        _notificationService = notificationService;
    }

    public async Task Handle(DomainEventNotification<TripBookingApprovedEvent> notification, CancellationToken cancellationToken)
    {
        var domainEvent = notification.DomainEvent;

        await _notificationService.CreateNotificationAsync(
            domainEvent.UserId,
            NotificationType.TripBookingApproved,
            "Booking approved!",
            "Your booking request has been approved. Get ready for your trip!",
            domainEvent.TripId,
            cancellationToken);
    }
}