using FishingHub.Mobile.Services.Interfaces;

namespace FishingHub.Mobile.Services.Implementations;

public class SecureTokenStorage : ISecureTokenStorage
{
    private const string AccessTokenKey = "access_token";
    private const string RefreshTokenKey = "refresh_token";
    private const string ExpiryKey = "access_token_expiry";

    public async Task SaveTokensAsync(string accessToken, string refreshToken, DateTime accessTokenExpiresOn)
    {
        await SecureStorage.Default.SetAsync(AccessTokenKey, accessToken);
        await SecureStorage.Default.SetAsync(RefreshTokenKey, refreshToken);
        await SecureStorage.Default.SetAsync(ExpiryKey, accessTokenExpiresOn.ToString("O")); // Round-trip format.
    }

    public async Task<string?> GetAccessTokenAsync()
    {
        return await TryGetAsync(AccessTokenKey);
    }

    public async Task<string?> GetRefreshTokenAsync()
    {
        return await TryGetAsync(RefreshTokenKey);
    }

    public async Task<DateTime?> GetAccessTokenExpiryAsync()
    {
        var raw = await TryGetAsync(ExpiryKey);
        return DateTime.TryParse(raw, null, System.Globalization.DateTimeStyles.RoundtripKind, out var parsed)
            ? parsed
            : null;
    }

    public Task ClearTokensAsync()
    {
        SecureStorage.Default.Remove(AccessTokenKey);
        SecureStorage.Default.Remove(RefreshTokenKey);
        SecureStorage.Default.Remove(ExpiryKey);

        return Task.CompletedTask;
    }

    private static async Task<string?> TryGetAsync(string key)
    {
        try
        {
            return await SecureStorage.Default.GetAsync(key);
        }
        catch
        {
            // SecureStorage can throw if the underlying platform keystore was reset
            // (e.g. app data cleared inconsistently) — treat as "no value" rather
            // than crashing the app.
            return null;
        }
    }
}