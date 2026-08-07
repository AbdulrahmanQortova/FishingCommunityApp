namespace FishingHub.Mobile.Models;

public class FlyoutMenuItem
{
    public string IconEmoji { get; set; } = string.Empty;
    public string TitleKey { get; set; } = string.Empty;
    public string Route { get; set; } = string.Empty;
    public bool HasBadge { get; set; }

    public static readonly FlyoutMenuItem ProfileShortcut = new()
    {
        IconEmoji = "👤",
        TitleKey = "Profile",
        Route = "profile"
    };
}