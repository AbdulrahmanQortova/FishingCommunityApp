using FishingCommunity.Application.Common.Interfaces;
using FishingCommunity.Shared.Wrappers;
using MediatR;

namespace FishingCommunity.Application.Features.Auth.Commands.ResendEmailVerification;

public class ResendEmailVerificationCommandHandler : IRequestHandler<ResendEmailVerificationCommand, Result>
{
    private readonly IIdentityService _identityService;
    private readonly IEmailService _emailService;

    public ResendEmailVerificationCommandHandler(IIdentityService identityService, IEmailService emailService)
    {
        _identityService = identityService;
        _emailService = emailService;
    }

    public async Task<Result> Handle(ResendEmailVerificationCommand request, CancellationToken cancellationToken)
    {
        var profile = await _identityService.GetUserProfileByEmailAsync(request.Email, cancellationToken);

        // Same anti-enumeration principle as ForgotPassword: always return a generic success message.
        if (profile is null || profile.IsEmailVerified)
        {
            return Result.Success("If your account requires verification, a new code has been sent.");
        }

        var newCodeResult = await _identityService.GenerateNewEmailVerificationCodeAsync(profile.UserId, cancellationToken);

        if (newCodeResult.Succeeded)
        {
            await _emailService.SendEmailVerificationAsync(request.Email, profile.FirstName, newCodeResult.Data!, cancellationToken);
        }

        return Result.Success("If your account requires verification, a new code has been sent.");
    }
}