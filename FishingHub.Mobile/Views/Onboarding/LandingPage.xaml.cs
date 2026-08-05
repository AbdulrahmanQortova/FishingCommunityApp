using FishingHub.Mobile.ViewModels.Onboarding;

namespace FishingHub.Mobile.Views.Onboarding;

public partial class LandingPage : ContentPage
{
    public LandingPage(LandingPageViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await PlayEntranceAnimationsAsync();
        StartAmbientAnimations();
    }

    protected override void OnDisappearing()
    {
        base.OnDisappearing();
        this.AbortAnimation("GlowPulse");
        this.AbortAnimation($"Float{Bubble1.GetHashCode()}");
        this.AbortAnimation($"Float{Bubble2.GetHashCode()}");
        this.AbortAnimation($"Float{Bubble3.GetHashCode()}");
        this.AbortAnimation($"Float{Bubble4.GetHashCode()}");
    }

    private async Task PlayEntranceAnimationsAsync()
    {
        IconCircle.Scale = 0.6;
        IconCircle.Opacity = 0;
        await Task.WhenAll(
            IconCircle.FadeToAsync(1, 500, Easing.CubicOut),
            IconCircle.ScaleToAsync(1, 600, Easing.SpringOut));

        TitleLabel.TranslationY = 15;
        await Task.WhenAll(
            TitleLabel.FadeToAsync(1, 400, Easing.CubicOut),
            TitleLabel.TranslateToAsync(0, 0, 400, Easing.CubicOut));

        await TaglineLabel.FadeToAsync(1, 300);

        SubtitleLabel.TranslationY = 10;
        await Task.WhenAll(
            SubtitleLabel.FadeToAsync(1, 400),
            SubtitleLabel.TranslateToAsync(0, 0, 400));

        DiveInButton.Scale = 0.9;
        await Task.WhenAll(
            DiveInButton.FadeToAsync(1, 400, Easing.CubicOut),
            DiveInButton.ScaleToAsync(1, 400, Easing.SpringOut));

        await SignInRow.FadeToAsync(1, 300);
    }

    private void StartAmbientAnimations()
    {
        // Breathing glow behind the icon — pulses outward and back continuously.
        var glowAnimation = new Animation();
        glowAnimation.Add(0, 0.5, new Animation(v => GlowCircle.Scale = v, 1.0, 1.15, Easing.SinInOut));
        glowAnimation.Add(0.5, 1, new Animation(v => GlowCircle.Scale = v, 1.15, 1.0, Easing.SinInOut));
        glowAnimation.Commit(this, "GlowPulse", length: 1600, repeat: () => true);

        // Gentle floating bubbles, each with a different speed for a natural feel.
        AnimateFloatingBubble(Bubble1, distance: 14, duration: 4200);
        AnimateFloatingBubble(Bubble2, distance: 10, duration: 3600);
        AnimateFloatingBubble(Bubble3, distance: 12, duration: 5000);
        AnimateFloatingBubble(Bubble4, distance: 16, duration: 4600);
    }

    private void AnimateFloatingBubble(VisualElement bubble, double distance, uint duration)
    {
        var animation = new Animation();
        animation.Add(0, 0.5, new Animation(v => bubble.TranslationY = v, 0, -distance, Easing.SinInOut));
        animation.Add(0.5, 1, new Animation(v => bubble.TranslationY = v, -distance, 0, Easing.SinInOut));

        animation.Commit(this, $"Float{bubble.GetHashCode()}", length: duration, repeat: () => true);
    }
}