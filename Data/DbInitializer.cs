using Microsoft.AspNetCore.Identity;
using SMS.Models;

namespace SMS.Data
{
    public static class DbInitializer
    {
        public static async Task SeedRolesAndAdminAsync(IServiceProvider service)
        {
            var userManager = service.GetRequiredService<UserManager<ApplicationUser>>();
            var roleManager = service.GetRequiredService<RoleManager<IdentityRole>>();

            // Seed Roles: Admin, ICT, Teacher, Parent
            string[] roleNames = { "Admin", "ICT", "Teacher", "Parent" };
            foreach (var roleName in roleNames)
            {
                if (!await roleManager.RoleExistsAsync(roleName))
                {
                    await roleManager.CreateAsync(new IdentityRole(roleName));
                }
            }

            // --- IMPROVED ADMIN SEEDING LOGIC ---
            var adminEmail = "gozmegatron@gmail.com";
            
            // Check if the admin user exists first
            var adminUser = await userManager.FindByEmailAsync(adminEmail);

            // If user does not exist, create them
            if (adminUser == null)
            {
                adminUser = new ApplicationUser
                {
                    UserName = adminEmail,
                    Email = adminEmail,
                    EmailConfirmed = true
                };
                await userManager.CreateAsync(adminUser, "Admin123!");
            }
            
            // Now, check if the user (whether newly created or existing) is in the "Admin" role.
            if (!await userManager.IsInRoleAsync(adminUser, "Admin"))
            {
                // If not, add them to the role.
                await userManager.AddToRoleAsync(adminUser, "Admin");
            }
        }
    }
}