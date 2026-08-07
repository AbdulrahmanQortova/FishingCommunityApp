using FishingHub.Mobile.ViewModels.AppShell; 
namespace FishingHub.Mobile;

public partial class MainAppShell : Shell
{
    public MainAppShell(AppShellViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;

        RegisterRoutes();
    }

    private static void RegisterRoutes()
    {
        Routing.RegisterRoute("orders", typeof(Views.Placeholder.PlaceholderPage));
        Routing.RegisterRoute("inventory", typeof(Views.Placeholder.PlaceholderPage));
        Routing.RegisterRoute("products", typeof(Views.Placeholder.PlaceholderPage));
        Routing.RegisterRoute("store-analytics", typeof(Views.Placeholder.PlaceholderPage));
        Routing.RegisterRoute("coupons", typeof(Views.Placeholder.PlaceholderPage));
        Routing.RegisterRoute("customers", typeof(Views.Placeholder.PlaceholderPage));
        Routing.RegisterRoute("my-boats", typeof(Views.Placeholder.PlaceholderPage));
        Routing.RegisterRoute("my-trips", typeof(Views.Placeholder.PlaceholderPage));
        Routing.RegisterRoute("booking-requests", typeof(Views.Placeholder.PlaceholderPage));
        Routing.RegisterRoute("trip-analytics", typeof(Views.Placeholder.PlaceholderPage));
        Routing.RegisterRoute("trips", typeof(Views.Placeholder.PlaceholderPage));
        Routing.RegisterRoute("shop", typeof(Views.Placeholder.PlaceholderPage));
        Routing.RegisterRoute("fishing-log", typeof(Views.Placeholder.PlaceholderPage));
        Routing.RegisterRoute("wishlist", typeof(Views.Placeholder.PlaceholderPage));
        Routing.RegisterRoute("tips", typeof(Views.Placeholder.PlaceholderPage));
        Routing.RegisterRoute("profile", typeof(Views.Placeholder.PlaceholderPage));
    }
}