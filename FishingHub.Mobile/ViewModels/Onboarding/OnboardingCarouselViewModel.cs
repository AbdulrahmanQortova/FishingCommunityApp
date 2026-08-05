using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using FishingHub.Mobile.Models;

namespace FishingHub.Mobile.ViewModels.Onboarding;

public partial class OnboardingCarouselViewModel : ObservableObject
{
    public List<OnboardingSlide> Slides { get; } = new()
    {
        new OnboardingSlide { Emoji = "🎣", TitleKey = "OnboardingTitle1", SubtitleKey = "OnboardingSubtitle1" },
        new OnboardingSlide { Emoji = "📍", TitleKey = "OnboardingTitle2", SubtitleKey = "OnboardingSubtitle2" },
        new OnboardingSlide { Emoji = "🚤", TitleKey = "OnboardingTitle3", SubtitleKey = "OnboardingSubtitle3" },
        new OnboardingSlide { Emoji = "🛍️", TitleKey = "OnboardingTitle4", SubtitleKey = "OnboardingSubtitle4" },
        new OnboardingSlide { Emoji = "💬", TitleKey = "OnboardingTitle5", SubtitleKey = "OnboardingSubtitle5" },
    };

    [ObservableProperty]
    private int currentIndex;

    public bool IsLastSlide => CurrentIndex == Slides.Count - 1;

    public string PageCounterText => $"{CurrentIndex + 1}/{Slides.Count}";

    partial void OnCurrentIndexChanged(int value)
    {
        OnPropertyChanged(nameof(IsLastSlide));
        OnPropertyChanged(nameof(PageCounterText));
    }

    [RelayCommand]
    private async Task SkipAsync()
    {
        await NavigateToAuthAsync();
    }

    [RelayCommand]
    private async Task NextOrGetStartedAsync()
    {
        if (IsLastSlide)
        {
            await NavigateToAuthAsync();
            return;
        }

        CurrentIndex++;
    }

    private static async Task NavigateToAuthAsync()
    {
        await Shell.Current.GoToAsync("role-selection");
    }
}