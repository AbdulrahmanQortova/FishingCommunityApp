using Microsoft.AspNetCore.Identity;

namespace FishingCommunity.Domain.Entities.Identity;

public class ApplicationRole : IdentityRole<Guid>
{
    public string? Description { get; set; }

    public ApplicationRole() : base() { }

    public ApplicationRole(string roleName) : base(roleName) { }
}