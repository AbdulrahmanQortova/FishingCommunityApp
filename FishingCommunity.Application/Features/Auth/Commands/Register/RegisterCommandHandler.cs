using FishingCommunity.Application.Common.Interfaces;
using FishingCommunity.Application.Common.Models;
using FishingCommunity.Domain.Events;
using FishingCommunity.Domain.Events.Identity;
using FishingCommunity.Shared.Wrappers;
using MediatR;

namespace FishingCommunity.Application.Features.Auth.Commands.Register;

public class RegisterCommandHandler : IRequestHandler<RegisterCommand, Result<RegisterResponse>>
{
    private readonly IIdentityService _identityService;
    private readonly IEmailService _emailService;
    private readonly IPublisher _publisher;

    public RegisterCommandHandler(
        IIdentityService identityService,
        IEmailService emailService,
        IPublisher publisher)
    {
        _identityService = identityService;
        _emailService = emailService;
        _publisher = publisher;
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

        // Raise domain event manually (ApplicationUser inherits IdentityUser<Guid>, not BaseEntity,
        // so it has no AddDomainEvent() mechanism — we publish explicitly here instead).
        var domainEvent = new UserRegisteredEvent(userId, request.Email);
        await _publisher.Publish(new DomainEventNotification<UserRegisteredEvent>(domainEvent), cancellationToken);

        var verificationCode = Guid.NewGuid().ToString("N")[..6].ToUpper();
        await _emailService.SendEmailVerificationAsync(request.Email, request.FirstName, verificationCode, cancellationToken);

        var response = new RegisterResponse
        {
            UserId = userId,
            Email = request.Email,
            Message = "Registration successful. Please check your email to verify your account."
        };

        return Result<RegisterResponse>.Success(response, response.Message);
    }
}