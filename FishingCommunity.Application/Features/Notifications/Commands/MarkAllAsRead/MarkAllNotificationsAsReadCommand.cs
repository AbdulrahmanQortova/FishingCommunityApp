using FishingCommunity.Shared.Wrappers;
using MediatR;

namespace FishingCommunity.Application.Features.Notifications.Commands.MarkAllAsRead;

public class MarkAllNotificationsAsReadCommand : IRequest<Result>
{
    public Guid UserId { get; set; }
}