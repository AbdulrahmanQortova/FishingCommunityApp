using CommunityToolkit.Maui;
using FishingHub.Mobile.Handlers;
using FishingHub.Mobile.Services.Implementations;
using FishingHub.Mobile.Services.Interfaces;
using FishingHub.Mobile.ViewModels.AppShell;
using FishingHub.Mobile.ViewModels.Auth;
using FishingHub.Mobile.ViewModels.Onboarding;
using FishingHub.Mobile.Views.Auth;
using FishingHub.Mobile.Views.Community;
using FishingHub.Mobile.Views.Onboarding;
using FishingHub.Mobile.Views.Placeholder;
using Microsoft.Extensions.Logging;

namespace FishingHub.Mobile;

public static class MauiProgram
{
    // Centralized here so both the HttpClient registration below and
    // TokenRefreshHandler's standalone refresh call use the exact same base URL.
    private const string ApiBaseUrl = "http://localhost:5296/"; // Android emulator.
    // Use "http://localhost:5296/" instead if targeting iOS Simulator / Windows.

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

        // ============================================================
        // Core services (Singleton — shared state across the whole app)
        // ============================================================
        builder.Services.AddSingleton<ILocalizationService, LocalizationService>();
        builder.Services.AddSingleton<IThemeService, ThemeService>();
        builder.Services.AddSingleton<ISecureTokenStorage, SecureTokenStorage>();
        builder.Services.AddSingleton<ICurrentUserService, CurrentUserService>();

        builder.Services.AddSingleton<App>();
        builder.Services.AddSingleton<AppShell>();
        builder.Services.AddSingleton<MainAppShell>();

        // ============================================================
        // HTTP infrastructure — AuthHeaderHandler injects the Bearer token on every
        // request; TokenRefreshHandler catches 401s, refreshes, and retries once.
        // Order matters: AuthHeaderHandler must run BEFORE TokenRefreshHandler so the
        // retried request also gets the freshly refreshed token attached.
        // ============================================================
        TokenRefreshHandler.InnerHandlerBaseAddress = new Uri(ApiBaseUrl);

        builder.Services.AddTransient<AuthHeaderHandler>();
        builder.Services.AddTransient<TokenRefreshHandler>();
        builder.Services.AddHttpClient<IApiClient, ApiClient>(client =>
        {
            client.BaseAddress = new Uri(ApiBaseUrl);
            client.Timeout = TimeSpan.FromSeconds(30);
        })
        .AddHttpMessageHandler<AuthHeaderHandler>()
        .AddHttpMessageHandler<TokenRefreshHandler>();

        builder.Services.AddScoped<IAuthApiService, AuthApiService>();

        // ============================================================
        // Pages & ViewModels (Transient — a fresh instance per navigation)
        // ============================================================
        builder.Services.AddTransient<LandingPageViewModel>();
        builder.Services.AddTransient<LandingPage>();

        builder.Services.AddTransient<OnboardingCarouselViewModel>();
        builder.Services.AddTransient<OnboardingCarouselPage>();

        builder.Services.AddTransient<AuthPlaceholderPage>();

        builder.Services.AddTransient<RoleSelectionViewModel>();
        builder.Services.AddTransient<RoleSelectionPage>();

        builder.Services.AddTransient<RegisterViewModel>();
        builder.Services.AddTransient<RegisterPage>();

        builder.Services.AddTransient<LoginViewModel>();
        builder.Services.AddTransient<LoginPage>();

        builder.Services.AddTransient<AppShellViewModel>();
        builder.Services.AddTransient<CommunityPage>();
        builder.Services.AddTransient<PlaceholderPage>();

#if DEBUG
        builder.Logging.AddDebug();
#endif

        var mauiApp = builder.Build();

        BorderlessEntryHandlerRegistration.Apply();

        return mauiApp;
    }
}