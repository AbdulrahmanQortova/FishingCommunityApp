using FishingCommunity.Application.Common.Interfaces;
using FishingCommunity.Shared.Wrappers;
using MediatR;

namespace FishingCommunity.Application.Features.Auth.Commands.ForgotPassword;

public class ForgotPasswordCommandHandler : IRequestHandler<ForgotPasswordCommand, Result>
{
    private readonly IIdentityService _identityService;
    private readonly IEmailService _emailService;

    public ForgotPasswordCommandHandler(IIdentityService identityService, IEmailService emailService)
    {
        _identityService = identityService;
        _emailService = emailService;
    }

    public async Task<Result> Handle(ForgotPasswordCommand request, CancellationToken cancellationToken)
    {
        var tokenResult = await _identityService.GeneratePasswordResetTokenAsync(request.Email, cancellationToken);

        // Security note: we always return Success here regardless of whether the email
        // exists in the system or not. This prevents user enumeration attacks — an attacker
        // shouldn't be able to tell which emails are registered by probing this endpoint.
        if (!tokenResult.Succeeded)
        {
            return Result.Success("If an account with that email exists, a password reset link has been sent.");
        }

        var profile = await _identityService.GetUserProfileByEmailAsync(request.Email, cancellationToken);

        if (profile is not null)
        {
            await _emailService.SendPasswordResetAsync(request.Email, profile.FirstName, tokenResult.Data!, cancellationToken);
        }

        return Result.Success("If an account with that email exists, a password reset link has been sent.");
    }
}