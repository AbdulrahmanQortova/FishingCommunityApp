using FishingCommunity.Domain.Entities.Notifications;
using FishingCommunity.Domain.Exceptions;
using FishingCommunity.Domain.Interfaces;
using FishingCommunity.Shared.Wrappers;
using MediatR;

namespace FishingCommunity.Application.Features.Notifications.Commands.MarkAsRead;

public class MarkNotificationAsReadCommandHandler : IRequestHandler<MarkNotificationAsReadCommand, Result>
{
    private readonly IUnitOfWork _unitOfWork;

    public MarkNotificationAsReadCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> Handle(MarkNotificationAsReadCommand request, CancellationToken cancellationToken)
    {
        var notification = await _unitOfWork.Repository<Notification>().GetByIdAsync(request.NotificationId, cancellationToken);

        if (notification is null)
        {
            throw new NotFoundException(nameof(Notification), request.NotificationId);
        }

        if (notification.RecipientUserId != request.UserId)
        {
            return Result.Failure("You are not authorized to modify this notification.");
        }

        notification.MarkAsRead();

        _unitOfWork.Repository<Notification>().Update(notification);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}