using Microsoft.AspNetCore.Identity;
using Work_request.Models;

namespace Work_request.Data;

public static class DbSeeder
{
    public static async Task SeedAdminAsync(IServiceProvider serviceProvider, IConfiguration configuration)
    {
        using var scope = serviceProvider.CreateScope();
        var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
        var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
        var logger = scope.ServiceProvider.GetRequiredService<ILoggerFactory>().CreateLogger(nameof(DbSeeder));

        const string adminRole = "Admin";
        if (!await roleManager.RoleExistsAsync(adminRole))
        {
            await roleManager.CreateAsync(new IdentityRole(adminRole));
        }

        const string userRole = "User";
        if (!await roleManager.RoleExistsAsync(userRole))
        {
            await roleManager.CreateAsync(new IdentityRole(userRole));
        }

        var adminEmail = configuration["AdminUser:Email"] ?? "admin@workrequest.local";
        var adminPassword = configuration["AdminUser:Password"] ?? "Admin@123456!";

        var adminUser = await userManager.FindByEmailAsync(adminEmail);
        if (adminUser is null)
        {
            adminUser = new ApplicationUser
            {
                UserName = adminEmail,
                Email = adminEmail,
                EmailConfirmed = true,
                FullName = "System Administrator"
            };

            var createResult = await userManager.CreateAsync(adminUser, adminPassword);
            if (createResult.Succeeded)
            {
                await userManager.AddToRoleAsync(adminUser, adminRole);
            }
        }
        else if (!await userManager.IsInRoleAsync(adminUser, adminRole))
        {
            await userManager.AddToRoleAsync(adminUser, adminRole);
        }

        const string standardUserEmail = "user@workrequest.local";
        const string standardUserPassword = "User@123456!";
        var standardUser = await userManager.FindByEmailAsync(standardUserEmail);
        if (standardUser is null)
        {
            standardUser = new ApplicationUser
            {
                UserName = standardUserEmail,
                Email = standardUserEmail,
                EmailConfirmed = true,
                FullName = "Standard Staff User"
            };

            var createResult = await userManager.CreateAsync(standardUser, standardUserPassword);
            if (createResult.Succeeded)
            {
                await userManager.AddToRoleAsync(standardUser, userRole);
            }
        }
        else if (!await userManager.IsInRoleAsync(standardUser, userRole))
        {
            await userManager.AddToRoleAsync(standardUser, userRole);
        }
    }
}
