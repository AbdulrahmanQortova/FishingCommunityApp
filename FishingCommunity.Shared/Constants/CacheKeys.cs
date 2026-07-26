namespace FishingCommunity.Shared.Constants;

public static class CacheKeys
{
    public const string WeatherPrefix = "weather:";
    public const string TripsPrefix = "trips:";
    public const string ProductsPrefix = "products:";
    public const string UserProfilePrefix = "user-profile:";

    public static string Weather(string locationKey) => $"{WeatherPrefix}{locationKey}";
    public static string Trip(Guid tripId) => $"{TripsPrefix}{tripId}";
    public static string Product(Guid productId) => $"{ProductsPrefix}{productId}";
    public static string UserProfile(Guid userId) => $"{UserProfilePrefix}{userId}";
}