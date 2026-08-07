using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using FishingHub.Mobile.Models;
using FishingHub.Mobile.Models.Api.Auth;
using FishingHub.Mobile.Services.Interfaces;

namespace FishingHub.Mobile.ViewModels.Auth;

public partial class LoginViewModel : ObservableObject
{
    private readonly IAuthApiService _authApiService;
    private readonly ISecureTokenStorage _tokenStorage;
    private readonly ICurrentUserService _currentUserService;
    private readonly ILocalizationService _localizationService;

    public LoginViewModel(
        IAuthApiService authApiService,
        ISecureTokenStorage tokenStorage,
        ICurrentUserService currentUserService,
        ILocalizationService localizationService)
    {
        _authApiService = authApiService;
        _tokenStorage = tokenStorage;
        _currentUserService = currentUserService;
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

            _currentUserService.SetUser(new CurrentUser
            {
                UserId = result.Data.UserId,
                FirstName = result.Data.FirstName,
                LastName = result.Data.LastName,
                Email = result.Data.Email,
                Roles = result.Data.Roles
            });

            // Swap the entire window content to the main app shell — the auth flow
            // (onboarding/login/register) is done, so there's no reason to keep it
            // on the navigation stack behind the main app.
            if (Application.Current is not null)
            {
                Application.Current.Windows[0].Page = IPlatformApplication.Current!.Services.GetRequiredService<MainAppShell>();
            }
        }
        finally
        {
            IsBusy = false;
        }
    }
}