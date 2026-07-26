namespace FishingCommunity.Application.Features.Auth.Commands.Logout;

public class LogoutRequestDto
{
    public string RefreshToken { get; set; } = string.Empty;
}