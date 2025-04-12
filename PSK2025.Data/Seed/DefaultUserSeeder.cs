using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using PSK2025.Models.Entities;

namespace PSK2025.Data.Seed
{
    public class DefaultUserSeeder : ISeeder
    {
        public async Task SeedAsync(IServiceProvider serviceProvider, ILogger logger)
        {
            using var scope = serviceProvider.CreateScope();
            var userManager = scope.ServiceProvider.GetRequiredService<UserManager<User>>();

            try
            {
                var adminEmail = "admin@example.com";
                var existingAdmin = await userManager.FindByEmailAsync(adminEmail);

                if (existingAdmin == null)
                {
                    var adminUser = new User
                    {
                        UserName = adminEmail,
                        Email = adminEmail,
                        EmailConfirmed = true,
                        FirstName = "Admin",
                        LastName = "User",
                    };

                    var result = await userManager.CreateAsync(adminUser, "SecurePassword123!");

                    if (result.Succeeded)
                    {
                        await userManager.AddToRoleAsync(adminUser, "Manager");

                        logger.LogInformation("Default admin user created successfully");
                    }
                    else
                    {
                        logger.LogError("Failed to create default admin: {Errors}",
                            string.Join(", ", result.Errors.Select(e => e.Description)));
                    }
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "An error occurred while seeding the default admin user");
            }
        }
    }
}