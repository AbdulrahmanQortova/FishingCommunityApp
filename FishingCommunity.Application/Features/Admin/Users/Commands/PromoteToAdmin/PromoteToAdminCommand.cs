using FishingCommunity.Shared.Wrappers;
using MediatR;

namespace FishingCommunity.Application.Features.Admin.Users.Commands.PromoteToAdmin;

public class PromoteToAdminCommand : IRequest<Result>
{
    public Guid UserId { get; set; }
}