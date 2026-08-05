using FishingHub.Mobile.ViewModels.Onboarding;

namespace FishingHub.Mobile.Views.Onboarding;

public partial class OnboardingCarouselPage : ContentPage
{
    public OnboardingCarouselPage(OnboardingCarouselViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }
}