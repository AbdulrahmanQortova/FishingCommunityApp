using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using FishingHub.Mobile.Models.Api.Auth;
using FishingHub.Mobile.Services.Interfaces;

namespace FishingHub.Mobile.ViewModels.Auth;

public partial class LoginViewModel : ObservableObject
{
    private readonly IAuthApiService _authApiService;
    private readonly ISecureTokenStorage _tokenStorage;
    private readonly ILocalizationService _localizationService;

    public LoginViewModel(IAuthApiService authApiService, ISecureTokenStorage tokenStorage, ILocalizationService localizationService)
    {
        _authApiService = authApiService;
        _tokenStorage = tokenStorage;
        _localizationService = localizationService;
    }

    [ObservableProperty]
    private string email = string.Empty;

    [ObservableProperty]
    private string password = string.Empty;

    [ObservableProperty]
    private bool isBusy;

    [ObservableProperty]
    private string errorMessage = string.Empty;

    [RelayCommand]
    private async Task GoBackAsync()
    {
        await Shell.Current.GoToAsync("..");
    }

    [RelayCommand]
    private async Task GoToRegisterAsync()
    {
        // No role was pre-selected from this entry point, so send the user through
        // the same role-selection step Register expects before it can proceed.
        await Shell.Current.GoToAsync("onboarding-carousel");
    }

    [RelayCommand]
    private async Task LoginAsync()
    {
        ErrorMessage = string.Empty;

        if (string.IsNullOrWhiteSpace(Email) || string.IsNullOrWhiteSpace(Password))
        {
            ErrorMessage = _localizationService.GetString("InvalidCredentials");
            return;
        }

        IsBusy = true;

        try
        {
            var request = new LoginRequest { Email = Email.Trim(), Password = Password };
            var result = await _authApiService.LoginAsync(request);

            if (!result.Succeeded || result.Data is null)
            {
                ErrorMessage = result.Errors.Length > 0
                    ? string.Join(" ", result.Errors)
                    : _localizationService.GetString("InvalidCredentials");
                return;
            }

            try
            {
                await _tokenStorage.SaveTokensAsync(
                    result.Data.AccessToken,
                    result.Data.RefreshToken,
                    result.Data.AccessTokenExpiresOn);
            }
            catch
            {
                ErrorMessage = _localizationService.GetString("SecureTokenError");
                return;
            }

            // Main app shell (Home/Trips/Community/Shop/Chat/Profile tabs) isn't built
            // yet — routes to the same placeholder for now, carrying the logged-in
            // user's basic info forward.
            await Shell.Current.GoToAsync("auth-placeholder", new Dictionary<string, object>
            {
                { "UserId", result.Data.UserId },
                { "FirstName", result.Data.FirstName }
            });
        }
        finally
        {
            IsBusy = false;
        }
    }
}