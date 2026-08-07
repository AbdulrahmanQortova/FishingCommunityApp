using FishingHub.Mobile.Services.Interfaces;

namespace FishingHub.Mobile;

public partial class App : Application
{
    private readonly ILocalizationService _localizationService;
    private readonly IThemeService _themeService;
    private readonly ICurrentUserService _currentUserService;

    public App(ILocalizationService localizationService, IThemeService themeService, ICurrentUserService currentUserService)
    {
        InitializeComponent();

        _localizationService = localizationService;
        _themeService = themeService;
        _currentUserService = currentUserService;

        _localizationService.PropertyChanged += OnLocalizationServicePropertyChanged;
    }

    protected override Window CreateWindow(IActivationState? activationState)
    {
        Page rootPage = new AppShell(); // Default: start at the auth flow.

        Task.Run(async () =>
        {
            await _localizationService.InitializeAsync();
            await _themeService.InitializeAsync();

            var hasSession = await _currentUserService.TryRestoreSessionAsync();

            if (hasSession)
            {
                rootPage = IPlatformApplication.Current!.Services.GetRequiredService<MainAppShell>();
            }
        }).GetAwaiter().GetResult();

        var window = new Window(rootPage);

        if (window.Page is not null)
        {
            window.Page.FlowDirection = _localizationService.CurrentFlowDirection;
        }

        return window;
    }

    private void OnLocalizationServicePropertyChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(ILocalizationService.CurrentFlowDirection))
        {
            ApplyFlowDirection();
        }
    }

    private void ApplyFlowDirection()
    {
        if (Windows.Count == 0) return;

        foreach (var window in Windows)
        {
            if (window.Page is not null)
            {
                window.Page.FlowDirection = _localizationService.CurrentFlowDirection;
            }
        }
    }
}