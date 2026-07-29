using FishingCommunity.Application.Common.Interfaces;
using FishingCommunity.Application.Common.Models;
using FishingCommunity.Domain.Entities.Trips;
using FishingCommunity.Domain.Enums;
using FishingCommunity.Domain.Events.Trips;
using FishingCommunity.Domain.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace FishingCommunity.Application.Features.Notifications.EventHandlers;

public class TripCancelledEventHandler : INotificationHandler<DomainEventNotification<TripCancelledEvent>>
{
    private readonly INotificationService _notificationService;
    private readonly IUnitOfWork _unitOfWork;

    public TripCancelledEventHandler(INotificationService notificationService, IUnitOfWork unitOfWork)
    {
        _notificationService = notificationService;
        _unitOfWork = unitOfWork;
    }

    public async Task Handle(DomainEventNotification<TripCancelledEvent> notification, CancellationToken cancellationToken)
    {
        var domainEvent = notification.DomainEvent;

        // Notify every user who had an active booking on this trip — not just the organizer.
        var affectedUserIds = await _unitOfWork.Repository<TripBooking>().Query()
            .Where(b => b.TripId == domainEvent.TripId)
            .Select(b => b.UserId)
            .Distinct()
            .ToListAsync(cancellationToken);

        foreach (var userId in affectedUserIds)
        {
            await _notificationService.CreateNotificationAsync(
                userId,
                NotificationType.TripCancelled,
                "Trip cancelled",
                domainEvent.Reason is not null
                    ? $"A trip you booked was cancelled. Reason: {domainEvent.Reason}"
                    : "A trip you booked was cancelled.",
                domainEvent.TripId,
                cancellationToken);
        }
    }
}