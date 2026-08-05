using FishingHub.Mobile.Services.Interfaces;

namespace FishingHub.Mobile;

public partial class App : Application
{
    private readonly ILocalizationService _localizationService;
    private readonly IThemeService _themeService;

    public App(ILocalizationService localizationService, IThemeService themeService)
    {
        InitializeComponent();

        _localizationService = localizationService;
        _themeService = themeService;

        _localizationService.PropertyChanged += OnLocalizationServicePropertyChanged;
    }

    protected override Window CreateWindow(IActivationState? activationState)
    {
        // Run on a background thread-pool thread (via Task.Run) instead of blocking the
        // UI thread directly. This avoids the classic sync-over-async deadlock: code
        // running here has no UI SynchronizationContext to marshal back onto, so the
        // internal awaits can complete freely, and blocking THIS thread for the result
        // is safe since it isn't the UI thread being waited on.
        Task.Run(async () =>
        {
            await _localizationService.InitializeAsync();
            await _themeService.InitializeAsync();
        }).GetAwaiter().GetResult();

        var window = new Window(new AppShell());

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