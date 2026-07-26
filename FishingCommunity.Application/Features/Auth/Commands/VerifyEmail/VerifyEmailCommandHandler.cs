using FishingCommunity.Application.Common.Interfaces;
using FishingCommunity.Application.Common.Models;
using FishingCommunity.Domain.Events.Identity;
using FishingCommunity.Shared.Wrappers;
using MediatR;

namespace FishingCommunity.Application.Features.Auth.Commands.VerifyEmail;

public class VerifyEmailCommandHandler : IRequestHandler<VerifyEmailCommand, Result>
{
    private readonly IIdentityService _identityService;
    private readonly IPublisher _publisher;

    public VerifyEmailCommandHandler(IIdentityService identityService, IPublisher publisher)
    {
        _identityService = identityService;
        _publisher = publisher;
    }

    public async Task<Result> Handle(VerifyEmailCommand request, CancellationToken cancellationToken)
    {
        var result = await _identityService.ConfirmEmailAsync(request.UserId, request.Code, cancellationToken);

        if (!result.Succeeded)
        {
            return Result.Failure(result.Errors);
        }

        var domainEvent = new UserEmailVerifiedEvent(request.UserId);
        await _publisher.Publish(new DomainEventNotification<UserEmailVerifiedEvent>(domainEvent), cancellationToken);

        return Result.Success("Email verified successfully.");
    }
}