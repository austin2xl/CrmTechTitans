using CrmTechTitans.Models.Enumerations;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace CrmTechTitans.Data
{
    public static class IdentityInitializer
    {
        public static async Task InitializeAsync(IServiceProvider serviceProvider)
        {
            var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
            var userManager = serviceProvider.GetRequiredService<UserManager<IdentityUser>>();
            
            // Create roles if they don't exist
            await CreateRolesAsync(roleManager);
            
            // Create test users for each role
            await CreateTestUsersAsync(userManager);
        }
        
        private static async Task CreateRolesAsync(RoleManager<IdentityRole> roleManager)
        {
            foreach (var roleName in UserRoles.AllRoles)
            {
                var roleExists = await roleManager.RoleExistsAsync(roleName);
                
                if (!roleExists)
                {
                    await roleManager.CreateAsync(new IdentityRole(roleName));
                }
            }
        }
        
        private static async Task CreateTestUsersAsync(UserManager<IdentityUser> userManager)
        {
            // Create a test user for Administrator role
            await CreateUserIfNotExistsAsync(userManager, "admin@crmtechtitans.com", "admin", UserRoles.Administrator);
            
            // Create a test user for Editor role
            await CreateUserIfNotExistsAsync(userManager, "editor@crmtechtitans.com", "editor", UserRoles.Editor);
            
            // Create a test user for ReadOnly role
            await CreateUserIfNotExistsAsync(userManager, "readonly@crmtechtitans.com", "readonly", UserRoles.ReadOnly);
        }
        
        private static async Task CreateUserIfNotExistsAsync(UserManager<IdentityUser> userManager, string email, string password, string role)
        {
            var user = await userManager.FindByEmailAsync(email);
            
            if (user == null)
            {
                var newUser = new IdentityUser
                {
                    UserName = email,
                    Email = email,
                    EmailConfirmed = true
                };
                
                var result = await userManager.CreateAsync(newUser, password);
                
                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(newUser, role);
                }
                else
                {
                    // Log the error
                    var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                    Console.WriteLine($"Error creating user {email}: {errors}");
                }
            }
        }
    }
} 