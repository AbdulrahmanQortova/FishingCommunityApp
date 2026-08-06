using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace FishingHub.Mobile.ViewModels.Onboarding;

public partial class LandingPageViewModel : ObservableObject
{
    [RelayCommand]
    private async Task DiveInAsync()
    {
        // Navigates into the two-page onboarding carousel with the Skip button.
        // The real carousel content is built in the next step — this route currently
        // points to a placeholder page just to prove the navigation flow works end-to-end.
        await Shell.Current.GoToAsync("onboarding-carousel");
    }

    [RelayCommand]
    private async Task SignInAsync()
    {
        await Shell.Current.GoToAsync("login");
    }
}