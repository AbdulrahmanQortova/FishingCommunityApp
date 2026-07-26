using FishingCommunity.Shared.Wrappers;
using MediatR;

namespace FishingCommunity.Application.Features.Auth.Commands.ResendEmailVerification;

public class ResendEmailVerificationCommand : IRequest<Result>
{
    public string Email { get; set; } = string.Empty;
}