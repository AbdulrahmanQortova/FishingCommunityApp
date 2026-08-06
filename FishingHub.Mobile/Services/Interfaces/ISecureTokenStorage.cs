namespace FishingHub.Mobile.Services.Interfaces;

public interface ISecureTokenStorage
{
    Task SaveTokensAsync(string accessToken, string refreshToken, DateTime accessTokenExpiresOn);
    Task<string?> GetAccessTokenAsync();
    Task<string?> GetRefreshTokenAsync();
    Task<DateTime?> GetAccessTokenExpiryAsync();
    Task ClearTokensAsync();
}