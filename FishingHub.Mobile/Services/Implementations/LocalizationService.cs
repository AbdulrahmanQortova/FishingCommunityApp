using FishingHub.Mobile.Models;
using FishingHub.Mobile.Services.Interfaces;
using System.ComponentModel;
using System.Text.Json;

namespace FishingHub.Mobile.Services.Implementations;

public class LocalizationService : ILocalizationService
{
    private const string LanguagePreferenceKey = "app_language";

    private Dictionary<string, string> _currentTranslations = new();
    private AppLanguage _currentLanguage = AppLanguage.English;

    public event PropertyChangedEventHandler? PropertyChanged;

    public AppLanguage CurrentLanguage => _currentLanguage;

    public FlowDirection CurrentFlowDirection => _currentLanguage.ToFlowDirection();

    public async Task InitializeAsync()
    {
        // Load the saved language preference, or default to the device's system
        // language if this is the first launch and it happens to be Arabic.
        var savedCode = Preferences.Default.Get(LanguagePreferenceKey, string.Empty);

        AppLanguage languageToLoad;

        if (!string.IsNullOrEmpty(savedCode))
        {
            languageToLoad = AppLanguageExtensions.FromCultureCode(savedCode);
        }
        else
        {
            var systemCulture = System.Globalization.CultureInfo.CurrentCulture.TwoLetterISOLanguageName;
            languageToLoad = systemCulture == "ar" ? AppLanguage.Arabic : AppLanguage.English;
        }

        await LoadTranslationsAsync(languageToLoad);
    }

    public async Task SetLanguageAsync(AppLanguage language)
    {
        if (language == _currentLanguage) return;

        await LoadTranslationsAsync(language);
        Preferences.Default.Set(LanguagePreferenceKey, language.ToCultureCode());
    }

    public string GetString(string key)
    {
        return _currentTranslations.TryGetValue(key, out var value) ? value : $"[{key}]";
    }
    public string ChevronGlyph => CurrentFlowDirection == FlowDirection.RightToLeft ? "‹" : "›";
    private async Task LoadTranslationsAsync(AppLanguage language)
    {
        var fileName = $"Localization/{language.ToCultureCode()}.json";

        try
        {
            using var stream = await FileSystem.OpenAppPackageFileAsync(fileName);
            using var reader = new StreamReader(stream);
            var json = await reader.ReadToEndAsync();

            var translations = JsonSerializer.Deserialize<Dictionary<string, string>>(json);

            _currentTranslations = translations ?? new Dictionary<string, string>();
            _currentLanguage = language;
        }
        catch (Exception ex)
        {
            // Never let a missing/broken translation file silently freeze the app on
            // "[key]" placeholders forever — log it clearly so the real cause (a wrong
            // file path, malformed JSON, etc.) is obvious during development.
            System.Diagnostics.Debug.WriteLine($"[LocalizationService] Failed to load '{fileName}': {ex}");
            _currentTranslations = new Dictionary<string, string>();
            _currentLanguage = language;
        }

        OnPropertyChanged(nameof(CurrentLanguage));
        OnPropertyChanged(nameof(CurrentFlowDirection));
        OnPropertyChanged(nameof(ChevronGlyph));
        OnPropertyChanged("Item[]");
    }

    private void OnPropertyChanged(string propertyName)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    // Indexer — enables {Binding [Welcome], Source={StaticResource Localization}}
    // style bindings directly in XAML as an alternative to the markup extension.
    public string this[string key] => GetString(key);
}