using FishingCommunity.Shared.Wrappers;
using MediatR;

namespace FishingCommunity.Application.Features.Auth.Commands.RefreshToken;

public class RefreshTokenCommand : IRequest<Result<RefreshTokenResponse>>
{
    public string AccessToken { get; set; } = string.Empty;
    public string RefreshToken { get; set; } = string.Empty;
    public string? IpAddress { get; set; }
}