using FishingHub.Mobile.Views.Auth;
using FishingHub.Mobile.Views.Onboarding;

namespace FishingHub.Mobile;

public partial class AppShell : Shell
{
    public AppShell()
    {
        InitializeComponent();

        Routing.RegisterRoute("onboarding-carousel", typeof(OnboardingCarouselPage));
        Routing.RegisterRoute("role-selection", typeof(RoleSelectionPage));
        Routing.RegisterRoute("auth-placeholder", typeof(AuthPlaceholderPage));
        Routing.RegisterRoute("register", typeof(RegisterPage));
        Routing.RegisterRoute("login", typeof(LoginPage));
    }
}