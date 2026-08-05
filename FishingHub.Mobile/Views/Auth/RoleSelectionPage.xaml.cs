using FishingHub.Mobile.ViewModels.Auth;

namespace FishingHub.Mobile.Views.Auth;

public partial class RoleSelectionPage : ContentPage
{
    public RoleSelectionPage(RoleSelectionViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }
}