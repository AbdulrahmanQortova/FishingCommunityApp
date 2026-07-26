namespace FishingCommunity.Shared.Extensions;

public static class DateTimeExtensions
{
    public static DateTime ToUtc(this DateTime dateTime)
        => dateTime.Kind == DateTimeKind.Utc
            ? dateTime
            : DateTime.SpecifyKind(dateTime, DateTimeKind.Utc);

    public static bool IsInPast(this DateTime dateTime)
        => dateTime < DateTime.UtcNow;

    public static bool IsInFuture(this DateTime dateTime)
        => dateTime > DateTime.UtcNow;

    public static string ToFriendlyDate(this DateTime dateTime)
        => dateTime.ToString("dd MMM yyyy, HH:mm");
}