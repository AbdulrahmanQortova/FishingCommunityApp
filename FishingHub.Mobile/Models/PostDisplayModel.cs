using CommunityToolkit.Mvvm.ComponentModel;

namespace FishingHub.Mobile.Models;

public partial class PostDisplayModel : ObservableObject
{
    public Guid Id { get; set; }
    public Guid AuthorId { get; set; }
    public string AuthorName { get; set; } = string.Empty;
    public string AuthorInitial { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    public List<string> PhotoUrls { get; set; } = new();
    public DateTime CreatedDate { get; set; }

    [ObservableProperty]
    private int commentsCount;

    [ObservableProperty]
    private int likesCount;

    [ObservableProperty]
    private string? currentUserReaction;

    public bool HasPhoto => PhotoUrls.Count > 0;
    public string? MainPhotoUrl => PhotoUrls.FirstOrDefault();
    public bool IsLikedByCurrentUser => CurrentUserReaction == "Like";

    public string LikeIconSource => IsLikedByCurrentUser ? "icon_thumb_up_filled.png" : "icon_thumb_up_outline.png";

    partial void OnCurrentUserReactionChanged(string? value)
    {
        OnPropertyChanged(nameof(IsLikedByCurrentUser));
        OnPropertyChanged(nameof(LikeIconSource));
    }
}