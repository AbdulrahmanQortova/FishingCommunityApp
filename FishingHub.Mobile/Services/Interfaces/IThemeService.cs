using FishingHub.Mobile.Models;

namespace FishingHub.Mobile.Services.Interfaces;

public interface IThemeService
{
    AppThemeMode CurrentTheme { get; }

    Task InitializeAsync();
    Task SetThemeAsync(AppThemeMode theme);
}