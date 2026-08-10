using CommunityToolkit.Mvvm.ComponentModel;

namespace FishingHub.Mobile.Models;

public partial class PostDisplayModel : ObservableObject
{
    public Guid Id { get; set; }
    public Guid AuthorId { get; set; }
    public string Content { get; set; } = string.Empty;
    public List<string> PhotoUrls { get; set; } = new();
    public DateTime CreatedDate { get; set; }
    public int CommentsCount { get; set; }

    [ObservableProperty]
    private int likesCount;

    [ObservableProperty]
    private string? currentUserReaction;

    public bool HasPhoto => PhotoUrls.Count > 0;
    public string? MainPhotoUrl => PhotoUrls.FirstOrDefault();

    public bool IsLikedByCurrentUser => CurrentUserReaction == "Like";
}