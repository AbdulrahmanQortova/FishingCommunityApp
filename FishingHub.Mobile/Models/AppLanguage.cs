namespace FishingHub.Mobile.Models;

public enum AppLanguage
{
    English,
    Arabic
}

public static class AppLanguageExtensions
{
    public static string ToCultureCode(this AppLanguage language) => language switch
    {
        AppLanguage.English => "en",
        AppLanguage.Arabic => "ar",
        _ => "en"
    };

    public static FlowDirection ToFlowDirection(this AppLanguage language) => language switch
    {
        AppLanguage.Arabic => FlowDirection.RightToLeft,
        _ => FlowDirection.LeftToRight
    };

    public static AppLanguage FromCultureCode(string code) => code switch
    {
        "ar" => AppLanguage.Arabic,
        _ => AppLanguage.English
    };
}