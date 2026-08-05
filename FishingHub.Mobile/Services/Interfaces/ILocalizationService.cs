using System.ComponentModel;
using FishingHub.Mobile.Models;

namespace FishingHub.Mobile.Services.Interfaces;

public interface ILocalizationService : INotifyPropertyChanged
{
    AppLanguage CurrentLanguage { get; }
    FlowDirection CurrentFlowDirection { get; }

    /// <summary>
    /// Gets the translated string for the given key in the current language.
    /// Returns the key itself (wrapped in brackets) if not found — makes missing
    /// translations obvious during development instead of failing silently.
    /// </summary>
    string GetString(string key);
    string ChevronGlyph { get; }
    Task InitializeAsync();
    Task SetLanguageAsync(AppLanguage language);
}