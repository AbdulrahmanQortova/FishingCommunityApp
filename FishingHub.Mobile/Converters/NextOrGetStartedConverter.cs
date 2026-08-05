using System.Globalization;
using FishingHub.Mobile.Services.Interfaces;

namespace FishingHub.Mobile.Converters;

public class NextOrGetStartedConverter : IValueConverter
{
    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        var localizationService = IPlatformApplication.Current?.Services.GetService<ILocalizationService>();

        var isLastSlide = value is bool b && b;
        var key = isLastSlide ? "GetStarted" : "Next";

        return localizationService?.GetString(key) ?? key;
    }

    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotSupportedException();
    }
}