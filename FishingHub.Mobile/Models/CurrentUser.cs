namespace FishingHub.Mobile.Models;

public class CurrentUser
{
    public Guid UserId { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public List<string> Roles { get; set; } = new();

    public string FullName => $"{FirstName} {LastName}".Trim();
    public string InitialLetter => string.IsNullOrEmpty(FirstName) ? "?" : FirstName[..1].ToUpperInvariant();

    public bool IsStoreOwner => Roles.Contains("StoreOwner");
    public bool IsBoatOwner => Roles.Contains("BoatOwner");
    public bool IsRegularUser => Roles.Contains("RegularUser");
    public bool IsAdministrator => Roles.Contains("Administrator");
}