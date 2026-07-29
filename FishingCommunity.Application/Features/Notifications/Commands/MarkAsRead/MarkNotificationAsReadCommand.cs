using FishingCommunity.Shared.Wrappers;
using MediatR;

namespace FishingCommunity.Application.Features.Notifications.Commands.MarkAsRead;

public class MarkNotificationAsReadCommand : IRequest<Result>
{
    public Guid NotificationId { get; set; }
    public Guid UserId { get; set; }
}