using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using FishingHub.Mobile.Models;
using FishingHub.Mobile.Models.Api.Auth;
using FishingHub.Mobile.Services.Interfaces;

namespace FishingHub.Mobile.ViewModels.Auth;

[QueryProperty(nameof(SelectedRole), "SelectedRole")]
public partial class RegisterViewModel : ObservableObject
{
    private readonly IAuthApiService _authApiService;
    private readonly ILocalizationService _localizationService;

    public RegisterViewModel(IAuthApiService authApiService, ILocalizationService localizationService)
    {
        _authApiService = authApiService;
        _localizationService = localizationService;
    }

    [ObservableProperty]
    private UserRole selectedRole = UserRole.RegularUser;

    partial void OnSelectedRoleChanged(UserRole value)
    {
        OnPropertyChanged(nameof(RoleBadgeKey));
        OnPropertyChanged(nameof(RoleIconEmoji));
    }

    public string RoleBadgeKey => SelectedRole switch
    {
        UserRole.BoatOwner => "RoleBadgeBoatOwner",
        UserRole.StoreOwner => "RoleBadgeStoreOwner",
        _ => "RoleBadgeRegularUser"
    };

    public string RoleIconEmoji => SelectedRole switch
    {
        UserRole.BoatOwner => "⛵",
        UserRole.StoreOwner => "🏪",
        _ => "🎧"
    };

    [ObservableProperty]
    private string fullName = string.Empty;

    [ObservableProperty]
    private string email = string.Empty;

    [ObservableProperty]
    private string phone = string.Empty;

    [ObservableProperty]
    private string password = string.Empty;

    [ObservableProperty]
    private string confirmPassword = string.Empty;

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
    private async Task CreateAccountAsync()
    {
        ErrorMessage = string.Empty;

        if (!ValidateInputs())
        {
            return;
        }

        IsBusy = true;

        try
        {
            var (firstName, lastName) = SplitFullName(FullName);

            var request = new RegisterRequest
            {
                FirstName = firstName,
                LastName = lastName,
                Email = Email.Trim(),
                Password = Password,
                ConfirmPassword = ConfirmPassword,
                Role = SelectedRole.ToString()
            };

            var result = await _authApiService.RegisterAsync(request);

            if (!result.Succeeded)
            {
                ErrorMessage = string.Join(" ", result.Errors);
                return;
            }

            // Email verification screen comes next — not built yet, so we navigate
            // to the same placeholder for now with the new user's id/email attached.
            await Shell.Current.GoToAsync("auth-placeholder", new Dictionary<string, object>
            {
                { "UserId", result.Data!.UserId },
                { "Email", result.Data.Email }
            });
        }
        finally
        {
            IsBusy = false;
        }
    }

    private bool ValidateInputs()
    {
        if (string.IsNullOrWhiteSpace(FullName))
        {
            ErrorMessage = _localizationService.GetString("FullNameRequired");
            return false;
        }

        if (string.IsNullOrWhiteSpace(Email))
        {
            ErrorMessage = _localizationService.GetString("EmailRequired");
            return false;
        }

        if (!Email.Contains('@') || !Email.Contains('.'))
        {
            ErrorMessage = _localizationService.GetString("EmailInvalid");
            return false;
        }

        if (Password.Length < 8)
        {
            ErrorMessage = _localizationService.GetString("PasswordTooShort");
            return false;
        }

        if (Password != ConfirmPassword)
        {
            ErrorMessage = _localizationService.GetString("PasswordsDoNotMatch");
            return false;
        }

        return true;
    }

    private static (string FirstName, string LastName) SplitFullName(string fullName)
    {
        var parts = fullName.Trim().Split(' ', 2, StringSplitOptions.RemoveEmptyEntries);

        return parts.Length switch
        {
            0 => (string.Empty, string.Empty),
            1 => (parts[0], string.Empty),
            _ => (parts[0], parts[1])
        };
    }
}