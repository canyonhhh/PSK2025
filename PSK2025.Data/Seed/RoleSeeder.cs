using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using PSK2025.Models.Enums;

namespace PSK2025.Data.Seed
{
    public class RoleSeeder : ISeeder
    {
        public async Task SeedAsync(IServiceProvider serviceProvider, ILogger logger)
        {
            using var scope = serviceProvider.CreateScope();
            var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();

            var roleNames = Enum.GetNames<UserRole>();

            foreach (var roleName in roleNames)
            {
                if (await roleManager.RoleExistsAsync(roleName))
                {
                    continue;
                }

                await roleManager.CreateAsync(new IdentityRole(roleName));
                logger.LogInformation("Created role: {RoleName}", roleName);
            }
        }
    }
}