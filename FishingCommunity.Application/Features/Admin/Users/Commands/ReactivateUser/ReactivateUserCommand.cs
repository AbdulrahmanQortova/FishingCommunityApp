using FishingCommunity.Shared.Wrappers;
using MediatR;

namespace FishingCommunity.Application.Features.Admin.Users.Commands.ReactivateUser;

public class ReactivateUserCommand : IRequest<Result>
{
    public Guid UserId { get; set; }
}