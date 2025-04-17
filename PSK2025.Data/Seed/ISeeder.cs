using Microsoft.Extensions.Logging;

namespace PSK2025.Data.Seed
{
    public interface ISeeder
    {
        Task SeedAsync(IServiceProvider serviceProvider, ILogger logger);
    }
}