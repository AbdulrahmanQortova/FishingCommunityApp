using FishingCommunity.Application.Common.Interfaces;
using FishingCommunity.Shared.Wrappers;
using MediatR;

namespace FishingCommunity.Application.Features.Admin.Users.Commands.ReactivateUser;

public class ReactivateUserCommandHandler : IRequestHandler<ReactivateUserCommand, Result>
{
    private readonly IIdentityService _identityService;

    public ReactivateUserCommandHandler(IIdentityService identityService)
    {
        _identityService = identityService;
    }

    public async Task<Result> Handle(ReactivateUserCommand request, CancellationToken cancellationToken)
    {
        return await _identityService.ReactivateUserAsync(request.UserId, cancellationToken);
    }
}