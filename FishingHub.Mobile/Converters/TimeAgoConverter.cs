using System.Globalization;
using FishingHub.Mobile.Services.Interfaces;

namespace FishingHub.Mobile.Converters;

public class TimeAgoConverter : IValueConverter
{
    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is not DateTime dateTime) return string.Empty;

        var localizationService = IPlatformApplication.Current?.Services.GetService<ILocalizationService>();
        if (localizationService is null) return string.Empty;

        var elapsed = DateTime.UtcNow - dateTime;

        if (elapsed.TotalMinutes < 1)
        {
            return localizationService.GetString("JustNow");
        }

        if (elapsed.TotalMinutes < 60)
        {
            return string.Format(localizationService.GetString("MinutesAgo"), (int)elapsed.TotalMinutes);
        }

        if (elapsed.TotalHours < 24)
        {
            return string.Format(localizationService.GetString("HoursAgo"), (int)elapsed.TotalHours);
        }

        return string.Format(localizationService.GetString("DaysAgo"), (int)elapsed.TotalDays);
    }

    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        => throw new NotSupportedException();
}