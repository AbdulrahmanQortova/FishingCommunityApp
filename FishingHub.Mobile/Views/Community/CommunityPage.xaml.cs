using FishingHub.Mobile.ViewModels.Community;

namespace FishingHub.Mobile.Views.Community;

public partial class CommunityPage : ContentPage
{
    private readonly CommunityViewModel _viewModel;
    private bool _hasLoadedOnce;

    public CommunityPage(CommunityViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
        _viewModel = viewModel;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();

        // Load the feed the first time this page appears — subsequent appearances
        // (e.g. navigating back from a post's comments) don't reload automatically,
        // since RefreshView's pull-to-refresh already covers manual refresh needs.
        if (!_hasLoadedOnce)
        {
            _hasLoadedOnce = true;
            await _viewModel.LoadFeedCommand.ExecuteAsync(null);
        }
    }
}