namespace FishingHub.Mobile.Models;

public class StoryItem
{
    public Guid UserId { get; set; }
    public string UserInitial { get; set; } = string.Empty;
    public string UserName { get; set; } = string.Empty;
    public string BackgroundColorHex { get; set; } = "#2A8FC7";
    public bool IsAddStoryPlaceholder { get; set; }
}