using System.Globalization;
using FishingHub.Mobile.Services.Interfaces;

namespace FishingHub.Mobile.Converters;

public class NextButtonTextConverter : IValueConverter
{
    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        var localizationService = IPlatformApplication.Current?.Services.GetService<ILocalizationService>();
        if (localizationService is null) return string.Empty;

        var isLastSlide = value is bool b && b;

        if (isLastSlide)
        {
            return localizationService.GetString("GetStarted");
        }

        var arrow = localizationService.CurrentFlowDirection == FlowDirection.RightToLeft ? "←" : "→";
        return $"{localizationService.GetString("Next")} {arrow}";
    }

    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        => throw new NotSupportedException();
}