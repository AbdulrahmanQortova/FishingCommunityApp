using FishingCommunity.Shared.Wrappers;
using MediatR;

namespace FishingCommunity.Application.Features.Admin.Users.Commands.SuspendUser;

public class SuspendUserCommand : IRequest<Result>
{
    public Guid UserId { get; set; }
}