using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using FishingHub.Mobile.Models;
using FishingHub.Mobile.Services.Interfaces;

namespace FishingHub.Mobile.ViewModels.Community;

public partial class CommunityViewModel : ObservableObject
{
    private readonly ICommunityApiService _communityApiService;
    private readonly ICurrentUserService _currentUserService;

    private int _currentPage = 1;
    private const int PageSize = 10;
    private bool _hasMorePages = true;

    public CommunityViewModel(ICommunityApiService communityApiService, ICurrentUserService currentUserService)
    {
        _communityApiService = communityApiService;
        _currentUserService = currentUserService;

        BuildMockStories();
    }

    public ObservableCollection<PostDisplayModel> Posts { get; } = new();

    // Stories are UI-only mock data for now — the backend has no Stories module.
    // Replace this with a real API-backed collection if/when that feature is built.
    public ObservableCollection<StoryItem> Stories { get; } = new();

    [ObservableProperty]
    private bool isBusy;

    [ObservableProperty]
    private bool isLoadingMore;

    [ObservableProperty]
    private bool isRefreshing;

    [ObservableProperty]
    private string newPostContent = string.Empty;

    [ObservableProperty]
    private bool isPosting;

    public string CurrentUserInitial => _currentUserService.User?.InitialLetter ?? "?";

    [RelayCommand]
    private async Task LoadFeedAsync()
    {
        if (IsBusy) return;

        IsBusy = true;
        _currentPage = 1;
        _hasMorePages = true;

        try
        {
            var result = await _communityApiService.GetFeedAsync(_currentPage, PageSize);
            System.Diagnostics.Debug.WriteLine($"Succeeded: {result.Succeeded}, Errors: {string.Join(",", result.Errors)}, ItemsCount: {result.Data?.Items.Count}");

            Posts.Clear();

            if (result.Succeeded && result.Data is not null)
            {
                foreach (var post in result.Data.Items)
                {
                    Posts.Add(MapToDisplayModel(post));
                }

                _hasMorePages = result.Data.HasNextPage;
            }
        }
        finally
        {
            IsBusy = false;
            IsRefreshing = false;
        }
    }

    [RelayCommand]
    private async Task RefreshAsync()
    {
        IsRefreshing = true;
        await LoadFeedAsync();
    }

    [RelayCommand]
    private async Task LoadMoreAsync()
    {
        if (IsLoadingMore || !_hasMorePages || IsBusy) return;

        IsLoadingMore = true;

        try
        {
            var nextPage = _currentPage + 1;
            var result = await _communityApiService.GetFeedAsync(nextPage, PageSize);

            if (result.Succeeded && result.Data is not null)
            {
                foreach (var post in result.Data.Items)
                {
                    Posts.Add(MapToDisplayModel(post));
                }

                _currentPage = nextPage;
                _hasMorePages = result.Data.HasNextPage;
            }
        }
        finally
        {
            IsLoadingMore = false;
        }
    }

    [RelayCommand]
    private async Task ToggleLikeAsync(PostDisplayModel post)
    {
        var wasLiked = post.IsLikedByCurrentUser;
        var previousCount = post.LikesCount;

        post.CurrentUserReaction = wasLiked ? null : "Like";
        post.LikesCount = wasLiked ? previousCount - 1 : previousCount + 1;

        var result = await _communityApiService.ReactToPostAsync(post.Id, "Like");

        if (!result.Succeeded)
        {
            post.CurrentUserReaction = wasLiked ? "Like" : null;
            post.LikesCount = previousCount;
        }
    }

    [RelayCommand]
    private async Task CreatePostAsync()
    {
        if (string.IsNullOrWhiteSpace(NewPostContent) || IsPosting) return;

        IsPosting = true;

        try
        {
            var request = new Models.Api.Community.CreatePostRequest { Content = NewPostContent.Trim() };
            var result = await _communityApiService.CreatePostAsync(request);

            if (result.Succeeded)
            {
                NewPostContent = string.Empty;
                await LoadFeedAsync();
            }
        }
        finally
        {
            IsPosting = false;
        }
    }

    private PostDisplayModel MapToDisplayModel(Models.Api.Community.PostSummary post)
    {
        return new PostDisplayModel
        {
            Id = post.Id,
            AuthorId = post.AuthorId,
            AuthorName = string.IsNullOrWhiteSpace(post.AuthorName) ? "Angler" : post.AuthorName,
            AuthorInitial = string.IsNullOrWhiteSpace(post.AuthorName) ? "?" : post.AuthorName[..1].ToUpperInvariant(),
            Content = post.Content,
            PhotoUrls = post.PhotoUrls,
            CreatedDate = post.CreatedDate,
            CommentsCount = post.CommentsCount,
            LikesCount = post.LikesCount,
            CurrentUserReaction = post.CurrentUserReaction
        };
    }

    private void BuildMockStories()
    {
        Stories.Add(new StoryItem { IsAddStoryPlaceholder = true, UserName = "YourStory" });
        Stories.Add(new StoryItem { UserInitial = "A", UserName = "Ahmed", BackgroundColorHex = "#2A8FC7" });
        Stories.Add(new StoryItem { UserInitial = "K", UserName = "Captain K", BackgroundColorHex = "#2CA58D" });
        Stories.Add(new StoryItem { UserInitial = "S", UserName = "Sara", BackgroundColorHex = "#C2478A" });
        Stories.Add(new StoryItem { UserInitial = "G", UserName = "Gulf Tk", BackgroundColorHex = "#D4A94A" });
    }
}