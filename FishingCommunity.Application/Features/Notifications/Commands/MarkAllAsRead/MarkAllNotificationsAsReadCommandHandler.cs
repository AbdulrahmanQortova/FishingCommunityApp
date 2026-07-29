using FishingCommunity.Domain.Entities.Notifications;
using FishingCommunity.Domain.Interfaces;
using FishingCommunity.Shared.Wrappers;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace FishingCommunity.Application.Features.Notifications.Commands.MarkAllAsRead;

public class MarkAllNotificationsAsReadCommandHandler : IRequestHandler<MarkAllNotificationsAsReadCommand, Result>
{
    private readonly IUnitOfWork _unitOfWork;

    public MarkAllNotificationsAsReadCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> Handle(MarkAllNotificationsAsReadCommand request, CancellationToken cancellationToken)
    {
        var unreadNotifications = await _unitOfWork.Repository<Notification>().Query()
            .Where(n => n.RecipientUserId == request.UserId && !n.IsRead)
            .ToListAsync(cancellationToken);

        foreach (var notification in unreadNotifications)
        {
            notification.MarkAsRead();
            _unitOfWork.Repository<Notification>().Update(notification);
        }

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success($"{unreadNotifications.Count} notification(s) marked as read.");
    }
}