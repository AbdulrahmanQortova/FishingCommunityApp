using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;

namespace FishingHub.Mobile.Models;

public partial class CommentDisplayModel : ObservableObject
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public string AuthorName { get; set; } = string.Empty;
    public string AuthorInitial { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    public DateTime CreatedDate { get; set; }
    public bool IsRemoved { get; set; }

    [ObservableProperty]
    private int likesCount;

    public ObservableCollection<CommentDisplayModel> Replies { get; set; } = new();

    public bool HasReplies => Replies.Count > 0;
}