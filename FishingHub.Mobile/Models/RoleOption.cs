namespace FishingHub.Mobile.Models;

public class RoleOption
{
    public UserRole Role { get; set; }
    public string IconEmoji { get; set; } = string.Empty;
    public string IconBackgroundHex { get; set; } = string.Empty;
    public string TitleKey { get; set; } = string.Empty;
    public string SubtitleKey { get; set; } = string.Empty;
}