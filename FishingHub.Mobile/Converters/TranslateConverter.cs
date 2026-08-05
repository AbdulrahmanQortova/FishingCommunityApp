using System.Globalization;
using FishingHub.Mobile.Services.Interfaces;

namespace FishingHub.Mobile.Converters;

public class TranslateConverter : IValueConverter
{
    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is not string key) return string.Empty;

        var localizationService = IPlatformApplication.Current?.Services.GetService<ILocalizationService>();
        return localizationService?.GetString(key) ?? key;
    }

    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotSupportedException();
    }
}