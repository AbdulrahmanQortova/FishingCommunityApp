using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using FishingHub.Mobile.Models;
using FishingHub.Mobile.Services.Interfaces;

namespace FishingHub.Mobile.ViewModels.AppShell; 

public partial class AppShellViewModel : ObservableObject
{
    private readonly ICurrentUserService _currentUserService;
    private readonly IThemeService _themeService;
    private readonly ILocalizationService _localizationService;

    public AppShellViewModel(
        ICurrentUserService currentUserService,
        IThemeService themeService,
        ILocalizationService localizationService)
    {
        _currentUserService = currentUserService;
        _themeService = themeService;
        _localizationService = localizationService;

        _currentUserService.PropertyChanged += (_, _) => RefreshMenu();

        RefreshMenu();
    }

    [ObservableProperty]
    private ObservableCollection<FlyoutMenuItem> primaryMenuItems = new();

    public string UserFullName => _currentUserService.User?.FullName ?? string.Empty;
    public string UserInitial => _currentUserService.User?.InitialLetter ?? "?";
    public string UserRoleDisplayKey => GetRoleDisplayKey();

    [RelayCommand]
    private async Task NavigateAsync(FlyoutMenuItem item)
    {
        Microsoft.Maui.Controls.Shell.Current.FlyoutIsPresented = false;
        await Microsoft.Maui.Controls.Shell.Current.GoToAsync($"//{item.Route}");
    }

    [RelayCommand]
    private async Task ToggleThemeAsync()
    {
        var newTheme = _themeService.CurrentTheme == AppThemeMode.Dark
            ? AppThemeMode.Light
            : AppThemeMode.Dark;

        await _themeService.SetThemeAsync(newTheme);
        OnPropertyChanged(nameof(ThemeToggleLabelKey));
    }

    public string ThemeToggleLabelKey => _themeService.CurrentTheme == AppThemeMode.Dark
        ? "LightMode"
        : "DarkMode";

    [RelayCommand]
    private async Task ChangeLanguageAsync()
    {
        var newLanguage = _localizationService.CurrentLanguage == AppLanguage.English
            ? AppLanguage.Arabic
            : AppLanguage.English;

        await _localizationService.SetLanguageAsync(newLanguage);
    }

    public string CurrentLanguageCode => _localizationService.CurrentLanguage == AppLanguage.English ? "EN" : "AR";

    [RelayCommand]
    private async Task LogoutAsync()
    {
        await _currentUserService.ClearAsync();
        await Microsoft.Maui.Controls.Shell.Current.GoToAsync("///landing");
    }

    private void RefreshMenu()
    {
        var user = _currentUserService.User;
        var items = new List<FlyoutMenuItem>
        {
            new() { IconEmoji = "🐟", TitleKey = "Community", Route = "community" }
        };

        if (user is not null)
        {
            if (user.IsStoreOwner)
            {
                items.AddRange(new[]
                {
                    new FlyoutMenuItem { IconEmoji = "📦", TitleKey = "Orders", Route = "orders" },
                    new FlyoutMenuItem { IconEmoji = "📋", TitleKey = "Inventory", Route = "inventory" },
                    new FlyoutMenuItem { IconEmoji = "🎧", TitleKey = "Products", Route = "products" },
                    new FlyoutMenuItem { IconEmoji = "📈", TitleKey = "Analytics", Route = "store-analytics" },
                    new FlyoutMenuItem { IconEmoji = "🏷️", TitleKey = "Coupons", Route = "coupons" },
                    new FlyoutMenuItem { IconEmoji = "👥", TitleKey = "Customers", Route = "customers" },
                });
            }
            else if (user.IsBoatOwner)
            {
                items.AddRange(new[]
                {
                    new FlyoutMenuItem { IconEmoji = "🚤", TitleKey = "MyBoats", Route = "my-boats" },
                    new FlyoutMenuItem { IconEmoji = "🗓️", TitleKey = "MyTrips", Route = "my-trips" },
                    new FlyoutMenuItem { IconEmoji = "📥", TitleKey = "BookingRequests", Route = "booking-requests" },
                    new FlyoutMenuItem { IconEmoji = "📈", TitleKey = "Analytics", Route = "trip-analytics" },
                });
            }
            else
            {
                items.AddRange(new[]
                {
                    new FlyoutMenuItem { IconEmoji = "🗓️", TitleKey = "Trips", Route = "trips" },
                    new FlyoutMenuItem { IconEmoji = "🛍️", TitleKey = "Shop", Route = "shop" },
                    new FlyoutMenuItem { IconEmoji = "📖", TitleKey = "FishingLog", Route = "fishing-log" },
                    new FlyoutMenuItem { IconEmoji = "❤️", TitleKey = "Wishlist", Route = "wishlist" },
                });
            }
        }

        items.Add(new FlyoutMenuItem { IconEmoji = "💡", TitleKey = "HelpfulTips", Route = "tips" });

        PrimaryMenuItems = new ObservableCollection<FlyoutMenuItem>(items);
        OnPropertyChanged(nameof(UserFullName));
        OnPropertyChanged(nameof(UserInitial));
        OnPropertyChanged(nameof(UserRoleDisplayKey));
    }

    private string GetRoleDisplayKey()
    {
        var user = _currentUserService.User;
        if (user is null) return string.Empty;

        if (user.IsStoreOwner) return "RoleBadgeStoreOwner";
        if (user.IsBoatOwner) return "RoleBadgeBoatOwner";
        return "RoleBadgeRegularUser";
    }
}