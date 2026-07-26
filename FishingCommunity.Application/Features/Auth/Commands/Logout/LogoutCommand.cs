using FishingCommunity.Shared.Wrappers;
using MediatR;

namespace FishingCommunity.Application.Features.Auth.Commands.Logout;

public class LogoutCommand : IRequest<Result>
{
    public Guid UserId { get; set; }
    public string RefreshToken { get; set; } = string.Empty;
    public string? IpAddress { get; set; }
}