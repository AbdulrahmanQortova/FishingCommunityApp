namespace FishingHub.Mobile.Models.Api.Auth;

public class RefreshTokenResponse
{
    public string AccessToken { get; set; } = string.Empty;
    public string RefreshToken { get; set; } = string.Empty;
    public DateTime AccessTokenExpiresOn { get; set; }
}