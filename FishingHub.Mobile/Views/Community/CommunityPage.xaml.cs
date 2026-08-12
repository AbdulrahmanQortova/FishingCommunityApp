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

        if (!_hasLoadedOnce)
        {
            _hasLoadedOnce = true;
            await _viewModel.LoadFeedCommand.ExecuteAsync(null);
        }
    }

    private void OnMenuClicked(object? sender, EventArgs e)
    {
        Shell.Current.FlyoutIsPresented = true;
    }
}