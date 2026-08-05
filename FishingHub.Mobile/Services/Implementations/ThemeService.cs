using FishingHub.Mobile.Models;
using FishingHub.Mobile.Services.Interfaces;

namespace FishingHub.Mobile.Services.Implementations;

public class ThemeService : IThemeService
{
    private const string ThemePreferenceKey = "app_theme";

    public AppThemeMode CurrentTheme { get; private set; } = AppThemeMode.Light;

    public Task InitializeAsync()
    {
        var savedTheme = Preferences.Default.Get(ThemePreferenceKey, nameof(AppThemeMode.System));
        var theme = Enum.TryParse<AppThemeMode>(savedTheme, out var parsed) ? parsed : AppThemeMode.System;

        ApplyTheme(theme);

        return Task.CompletedTask;
    }

    public Task SetThemeAsync(AppThemeMode theme)
    {
        Preferences.Default.Set(ThemePreferenceKey, theme.ToString());
        ApplyTheme(theme);

        return Task.CompletedTask;
    }

    private void ApplyTheme(AppThemeMode theme)
    {
        CurrentTheme = theme;

        // Resolve "System" down to an actual Light/Dark based on the OS setting.
        var effectiveTheme = theme == AppThemeMode.System
            ? (Application.Current!.RequestedTheme == Microsoft.Maui.ApplicationModel.AppTheme.Dark ? AppThemeMode.Dark : AppThemeMode.Light)
            : theme;

        var resources = Application.Current!.Resources;

        if (effectiveTheme == AppThemeMode.Dark)
        {
            resources["PageBackgroundColor"] = resources["DarkBackground"];
            resources["SurfaceColor"] = resources["DarkSurface"];
            resources["TextPrimaryColor"] = resources["DarkTextPrimary"];
            resources["TextSecondaryColor"] = resources["DarkTextSecondary"];
            resources["BorderColor"] = resources["DarkBorder"];
        }
        else
        {
            resources["PageBackgroundColor"] = resources["LightBackground"];
            resources["SurfaceColor"] = resources["LightSurface"];
            resources["TextPrimaryColor"] = resources["LightTextPrimary"];
            resources["TextSecondaryColor"] = resources["LightTextSecondary"];
            resources["BorderColor"] = resources["LightBorder"];
        }

        // Also set the native platform theme (affects system UI elements like the
        // status bar, native dialogs, etc.) to match.
        Application.Current.UserAppTheme = effectiveTheme == AppThemeMode.Dark
            ? Microsoft.Maui.ApplicationModel.AppTheme.Dark
            : Microsoft.Maui.ApplicationModel.AppTheme.Light;
    }
}