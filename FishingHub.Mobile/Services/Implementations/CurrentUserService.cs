using System.ComponentModel;
using System.Text.Json;
using FishingHub.Mobile.Models;
using FishingHub.Mobile.Services.Interfaces;

namespace FishingHub.Mobile.Services.Implementations;

public class CurrentUserService : ICurrentUserService
{
    private const string CurrentUserStorageKey = "current_user_profile";

    private readonly ISecureTokenStorage _tokenStorage;

    public event PropertyChangedEventHandler? PropertyChanged;

    public CurrentUser? User { get; private set; }

    public bool IsAuthenticated => User is not null;

    public CurrentUserService(ISecureTokenStorage tokenStorage)
    {
        _tokenStorage = tokenStorage;
    }

    public void SetUser(CurrentUser user)
    {
        User = user;

        // Persist the profile (non-sensitive display info) so it survives app restarts
        // without needing a fresh API call — the actual auth tokens are stored
        // separately and securely via ISecureTokenStorage.
        var json = JsonSerializer.Serialize(user);
        Preferences.Default.Set(CurrentUserStorageKey, json);

        OnPropertyChanged(nameof(User));
        OnPropertyChanged(nameof(IsAuthenticated));
    }

    public async Task ClearAsync()
    {
        User = null;
        Preferences.Default.Remove(CurrentUserStorageKey);
        await _tokenStorage.ClearTokensAsync();

        OnPropertyChanged(nameof(User));
        OnPropertyChanged(nameof(IsAuthenticated));
    }

    public async Task<bool> TryRestoreSessionAsync()
    {
        var accessToken = await _tokenStorage.GetAccessTokenAsync();

        if (string.IsNullOrEmpty(accessToken))
        {
            return false;
        }

        var json = Preferences.Default.Get(CurrentUserStorageKey, string.Empty);

        if (string.IsNullOrEmpty(json))
        {
            return false;
        }

        try
        {
            var user = JsonSerializer.Deserialize<CurrentUser>(json);

            if (user is null) return false;

            User = user;
            OnPropertyChanged(nameof(User));
            OnPropertyChanged(nameof(IsAuthenticated));

            return true;
        }
        catch
        {
            return false;
        }
    }

    private void OnPropertyChanged(string propertyName)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}