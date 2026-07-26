using FishingCommunity.Domain.Entities.Identity;
using FishingCommunity.Domain.Enums;
using FishingCommunity.Shared.Constants;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace FishingCommunity.Infrastructure.Persistence;

public static class ApplicationDbContextSeed
{
    public static async Task SeedAsync(
        UserManager<ApplicationUser> userManager,
        RoleManager<ApplicationRole> roleManager,
        IConfiguration configuration,
        ILogger logger)
    {
        await SeedRolesAsync(roleManager, logger);
        await SeedDefaultAdminAsync(userManager, configuration, logger);
    }

    private static async Task SeedRolesAsync(RoleManager<ApplicationRole> roleManager, ILogger logger)
    {
        string[] roles =
        {
            Roles.RegularUser,
            Roles.BoatOwner,
            Roles.StoreOwner,
            Roles.Administrator
        };

        foreach (var roleName in roles)
        {
            if (!await roleManager.RoleExistsAsync(roleName))
            {
                var result = await roleManager.CreateAsync(new ApplicationRole(roleName)
                {
                    Description = $"{roleName} role"
                });

                if (result.Succeeded)
                {
                    logger.LogInformation("Seeded role: {RoleName}", roleName);
                }
                else
                {
                    logger.LogError("Failed to seed role {RoleName}: {Errors}",
                        roleName, string.Join(", ", result.Errors.Select(e => e.Description)));
                }
            }
        }
    }

    private static async Task SeedDefaultAdminAsync(
        UserManager<ApplicationUser> userManager,
        IConfiguration configuration,
        ILogger logger)
    {
        var adminEmail = configuration["SeedData:AdminEmail"];
        var adminPassword = configuration["SeedData:AdminPassword"];

        if (string.IsNullOrWhiteSpace(adminEmail) || string.IsNullOrWhiteSpace(adminPassword))
        {
            logger.LogWarning("SeedData:AdminEmail / SeedData:AdminPassword not configured — skipping default admin seed.");
            return;
        }

        var existingAdmin = await userManager.FindByEmailAsync(adminEmail);
        if (existingAdmin is not null)
        {
            return; // Already seeded.
        }

        var adminUser = new ApplicationUser
        {
            UserName = adminEmail,
            Email = adminEmail,
            FirstName = "System",
            LastName = "Administrator",
            EmailConfirmed = true,
            IsEmailVerified = true,
            Status = AccountStatus.Active,
            CreatedDate = DateTime.UtcNow
        };

        var createResult = await userManager.CreateAsync(adminUser, adminPassword);

        if (!createResult.Succeeded)
        {
            logger.LogError("Failed to seed default admin: {Errors}",
                string.Join(", ", createResult.Errors.Select(e => e.Description)));
            return;
        }

        await userManager.AddToRoleAsync(adminUser, Roles.Administrator);
        logger.LogInformation("Seeded default administrator account: {Email}", adminEmail);
    }
}