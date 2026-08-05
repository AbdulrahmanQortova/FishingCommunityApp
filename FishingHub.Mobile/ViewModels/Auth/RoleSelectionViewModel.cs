using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using FishingHub.Mobile.Models;

namespace FishingHub.Mobile.ViewModels.Auth;

public partial class RoleSelectionViewModel : ObservableObject
{
    public List<RoleOption> Roles { get; } = new()
    {
        new RoleOption
        {
            Role = UserRole.RegularUser,
            IconEmoji = "🎧",
            IconBackgroundHex = "#2A8FC7",
            TitleKey = "RoleRegularUserTitle",
            SubtitleKey = "RoleRegularUserSubtitle"
        },
        new RoleOption
        {
            Role = UserRole.BoatOwner,
            IconEmoji = "⛵",
            IconBackgroundHex = "#2A8FC7",
            TitleKey = "RoleBoatOwnerTitle",
            SubtitleKey = "RoleBoatOwnerSubtitle"
        },
        new RoleOption
        {
            Role = UserRole.StoreOwner,
            IconEmoji = "🏪",
            IconBackgroundHex = "#2A8FC7",
            TitleKey = "RoleStoreOwnerTitle",
            SubtitleKey = "RoleStoreOwnerSubtitle"
        }
    };

    [RelayCommand]
    private async Task SelectRoleAsync(RoleOption selectedRole)
    {
        await Shell.Current.GoToAsync("register", new Dictionary<string, object>
    {
        { "SelectedRole", selectedRole.Role }
    });
    }
}