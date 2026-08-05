namespace FishingHub.Mobile.Models;

// Values match the backend's Shared.Constants.Roles exactly (RegularUser, BoatOwner, StoreOwner)
// so the selected role can be sent to the Register endpoint without any translation layer.
public enum UserRole
{
    RegularUser,
    BoatOwner,
    StoreOwner
}