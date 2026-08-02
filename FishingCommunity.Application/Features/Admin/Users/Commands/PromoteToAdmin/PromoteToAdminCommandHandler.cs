using FishingCommunity.Application.Common.Interfaces;
using FishingCommunity.Shared.Wrappers;
using MediatR;

namespace FishingCommunity.Application.Features.Admin.Users.Commands.PromoteToAdmin;

public class PromoteToAdminCommandHandler : IRequestHandler<PromoteToAdminCommand, Result>
{
    private readonly IIdentityService _identityService;

    public PromoteToAdminCommandHandler(IIdentityService identityService)
    {
        _identityService = identityService;
    }

    public async Task<Result> Handle(PromoteToAdminCommand request, CancellationToken cancellationToken)
    {
        return await _identityService.PromoteToAdminAsync(request.UserId, cancellationToken);
    }
}