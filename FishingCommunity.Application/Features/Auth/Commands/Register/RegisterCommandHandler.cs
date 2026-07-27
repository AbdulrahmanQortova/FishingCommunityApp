using FishingCommunity.Application.Common.Interfaces;
using FishingCommunity.Application.Common.Models;
using FishingCommunity.Domain.Events.Identity;
using FishingCommunity.Shared.Wrappers;
using MediatR;
using Microsoft.Extensions.Options;

namespace FishingCommunity.Application.Features.Auth.Commands.Register;

public class RegisterCommandHandler : IRequestHandler<RegisterCommand, Result<RegisterResponse>>
{
    private readonly IIdentityService _identityService;
    private readonly IEmailService _emailService;
    private readonly IPublisher _publisher;
    private readonly FeatureFlags _featureFlags;

    public RegisterCommandHandler(
        IIdentityService identityService,
        IEmailService emailService,
        IPublisher publisher,
        IOptions<FeatureFlags> featureFlags)
    {
        _identityService = identityService;
        _emailService = emailService;
        _publisher = publisher;
        _featureFlags = featureFlags.Value;
    }

    public async Task<Result<RegisterResponse>> Handle(RegisterCommand request, CancellationToken cancellationToken)
    {
        var createResult = await _identityService.CreateUserAsync(
            request.Email,
            request.Password,
            request.FirstName,
            request.LastName,
            request.Role,
            cancellationToken);

        if (!createResult.Succeeded)
        {
            return Result<RegisterResponse>.Failure(createResult.Errors);
        }

        var userId = createResult.Data;

        var domainEvent = new UserRegisteredEvent(userId, request.Email);
        await _publisher.Publish(new Common.Models.DomainEventNotification<UserRegisteredEvent>(domainEvent), cancellationToken);

        string message;

        if (_featureFlags.RequireEmailVerification)
        {
            var codeResult = await _identityService.GenerateNewEmailVerificationCodeAsync(userId, cancellationToken);

            if (codeResult.Succeeded)
            {
                await _emailService.SendEmailVerificationAsync(request.Email, request.FirstName, codeResult.Data!, cancellationToken);
            }

            message = "Registration successful. Please check your email to verify your account.";
        }
        else
        {
            // Email verification disabled via feature flag — mark the account as verified immediately.
            await _identityService.ConfirmEmailAsync(userId, string.Empty, cancellationToken, bypassCodeCheck: true);
            message = "Registration successful. You can now log in.";
        }

        var response = new RegisterResponse
        {
            UserId = userId,
            Email = request.Email,
            Message = message
        };

        return Result<RegisterResponse>.Success(response, message);
    }
}