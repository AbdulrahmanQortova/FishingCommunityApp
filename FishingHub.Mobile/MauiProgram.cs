using CommunityToolkit.Maui;
using FishingHub.Mobile.Handlers;
using FishingHub.Mobile.Services.Implementations;
using FishingHub.Mobile.Services.Interfaces;
using FishingHub.Mobile.ViewModels.Auth;
using FishingHub.Mobile.ViewModels.Onboarding;
using FishingHub.Mobile.Views.Auth;
using FishingHub.Mobile.Views.Onboarding;
using Microsoft.Extensions.Logging;

namespace FishingHub.Mobile;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();

        builder
            .UseMauiApp<App>()
            .UseMauiCommunityToolkit()
            .ConfigureFonts(fonts =>
            {
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
            });

        // --- Core services (Singleton — shared state across the whole app) ---
        builder.Services.AddSingleton<ILocalizationService, LocalizationService>();
        builder.Services.AddSingleton<IThemeService, ThemeService>();
        builder.Services.AddSingleton<App>();
        builder.Services.AddSingleton<AppShell>();

        // --- Pages & ViewModels (Transient — a fresh instance per navigation) ---
        builder.Services.AddTransient<LandingPageViewModel>();
        builder.Services.AddTransient<LandingPage>();
        builder.Services.AddTransient<OnboardingCarouselPlaceholderPage>();
        builder.Services.AddTransient<OnboardingCarouselViewModel>();
        builder.Services.AddTransient<OnboardingCarouselPage>();
        builder.Services.AddTransient<AuthPlaceholderPage>();
        builder.Services.AddTransient<RoleSelectionViewModel>();
        builder.Services.AddTransient<RoleSelectionPage>();
        builder.Services.AddTransient<RegisterViewModel>();
        builder.Services.AddTransient<RegisterPage>();
        BorderlessEntryHandlerRegistration.Apply();


        builder.Services.AddSingleton<IAuthApiService>(sp =>
        {
            var httpClient = new HttpClient
            {
                BaseAddress = new Uri(DeviceInfo.Platform == DevicePlatform.Android
                    ? "http://10.0.2.2:5296/"
                    : "http://localhost:5296/"),
                Timeout = TimeSpan.FromSeconds(30)
            };

            return new AuthApiService(httpClient);
        });

#if DEBUG
        builder.Logging.AddDebug();
#endif

        return builder.Build();
    }
}