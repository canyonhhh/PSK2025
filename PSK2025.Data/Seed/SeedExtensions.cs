using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace PSK2025.Data.Seed
{
    public static class SeedExtensions
    {
        public static async Task SeedDataAsync(this IServiceProvider serviceProvider)
        {
            using var scope = serviceProvider.CreateScope();
            var loggerFactory = scope.ServiceProvider.GetRequiredService<ILoggerFactory>();
            var logger = loggerFactory.CreateLogger("DataSeeding");
            
            logger.LogInformation("Starting data seeding...");
            
            // Get all registered seeders
            var seeders = scope.ServiceProvider.GetServices<ISeeder>();
            
            // Run each seeder
            foreach (var seeder in seeders)
            {
                await seeder.SeedAsync(serviceProvider, logger);
            }
            
            logger.LogInformation("Data seeding completed");
        }
        
        // Extension method to register all seeders
        public static IServiceCollection AddDataSeeders(this IServiceCollection services)
        {
            // Register your seeders
            services.AddTransient<ISeeder, RoleSeeder>();
            services.AddTransient<ISeeder, DefaultUserSeeder>();
            
            return services;
        }
    }
}