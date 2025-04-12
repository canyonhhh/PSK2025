using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using PSK2025.Data.Seed.Models;
using PSK2025.Models.Entities;

namespace PSK2025.Data.Seed
{
    public class DefaultUserSeeder : ISeeder
    {
        public async Task SeedAsync(IServiceProvider serviceProvider, ILogger logger)
        {
            using var scope = serviceProvider.CreateScope();
            var userManager = scope.ServiceProvider.GetRequiredService<UserManager<User>>();
            var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
            var configuration = scope.ServiceProvider.GetRequiredService<IConfiguration>();

            try
            {
                var userSeedData = new UserSeedData
                {
                    Users = new List<UserAccount>()
                };
                
                logger.LogInformation($"configuration: {configuration.GetSection("Users").Value}");

                configuration.GetSection("Users").Bind(userSeedData.Users);

                if (userSeedData.Users.Count == 0)
                {
                    logger.LogWarning("No user accounts found in configuration");
                    return;
                }

                foreach (var userAccount in userSeedData.Users)
                {
                    if (!await roleManager.RoleExistsAsync(userAccount.Role))
                    {
                        logger.LogWarning("Skipping user {Email} because role {Role} does not exist", 
                            userAccount.Email, userAccount.Role);
                        continue;
                    }

                    var existingUser = await userManager.FindByEmailAsync(userAccount.Email);

                    if (existingUser == null)
                    {
                        var user = new User
                        {
                            UserName = userAccount.Email,
                            Email = userAccount.Email,
                            EmailConfirmed = true,
                            FirstName = userAccount.FirstName,
                            LastName = userAccount.LastName,
                        };

                        var result = await userManager.CreateAsync(user, userAccount.Password);

                        if (result.Succeeded)
                        {
                            await userManager.AddToRoleAsync(user, userAccount.Role);
                            logger.LogInformation("User {Email} created successfully with role {Role}", 
                                userAccount.Email, userAccount.Role);
                        }
                        else
                        {
                            logger.LogError("Failed to create user {Email}: {Errors}",
                                userAccount.Email, string.Join(", ", result.Errors.Select(e => e.Description)));
                        }
                    }
                    else
                    {
                        logger.LogInformation("User {Email} already exists", userAccount.Email);
                    }
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "An error occurred while seeding users from configuration");
            }
        }
    }
}