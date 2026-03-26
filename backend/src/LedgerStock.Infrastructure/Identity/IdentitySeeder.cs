using LedgerStock.Domain.Constants;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;

namespace LedgerStock.Infrastructure.Identity;

public static class IdentitySeeder
{
    public static async Task SeedAsync(IServiceProvider serviceProvider)
    {
        using var scope = serviceProvider.CreateScope();

        var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
        var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();

        foreach (var role in SystemRoles.All)
        {
            var exists = await roleManager.RoleExistsAsync(role);
            if (!exists)
            {
                await roleManager.CreateAsync(new IdentityRole(role));
            }
        }

        var masterEmail = "master@ledgerstock.com";
        var masterUser = await userManager.FindByEmailAsync(masterEmail);

        if (masterUser is null)
        {
            masterUser = new ApplicationUser
            {
                FullName = "Master User",
                UserName = masterEmail,
                Email = masterEmail,
                EmailConfirmed = true
            };

            var result = await userManager.CreateAsync(masterUser, "Master123!");

            if (result.Succeeded)
            {
                await userManager.AddToRoleAsync(masterUser, SystemRoles.Master);
            }
        }
        else
        {
            var roles = await userManager.GetRolesAsync(masterUser);
            if (!roles.Contains(SystemRoles.Master))
            {
                await userManager.AddToRoleAsync(masterUser, SystemRoles.Master);
            }
        }
    }
}