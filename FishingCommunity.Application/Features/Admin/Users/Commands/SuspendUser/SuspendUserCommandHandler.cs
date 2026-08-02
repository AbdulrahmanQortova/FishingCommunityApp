using FishingCommunity.Application.Common.Interfaces;
using FishingCommunity.Shared.Wrappers;
using MediatR;

namespace FishingCommunity.Application.Features.Admin.Users.Commands.SuspendUser;

public class SuspendUserCommandHandler : IRequestHandler<SuspendUserCommand, Result>
{
    private readonly IIdentityService _identityService;

    public SuspendUserCommandHandler(IIdentityService identityService)
    {
        _identityService = identityService;
    }

    public async Task<Result> Handle(SuspendUserCommand request, CancellationToken cancellationToken)
    {
        return await _identityService.SuspendUserAsync(request.UserId, cancellationToken);
    }
}