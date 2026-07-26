using FishingCommunity.Shared.Wrappers;
using MediatR;

namespace FishingCommunity.Application.Features.Auth.Commands.VerifyEmail;

public class VerifyEmailCommand : IRequest<Result>
{
    public Guid UserId { get; set; }
    public string Code { get; set; } = string.Empty;
}