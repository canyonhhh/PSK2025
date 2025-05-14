using Microsoft.EntityFrameworkCore;
using PSK2025.Data.Contexts;
using PSK2025.Data.Repositories.Interfaces;
using PSK2025.Models.Entities;
using System.Threading.Tasks;

namespace PSK2025.Data.Repositories
{
    public class AppSettingsRepository(AppDbContext dbContext) : IAppSettingsRepository
    {
        private readonly AppDbContext _dbContext = dbContext;
        private const int SettingsId = 1;

        public async Task<AppSettings?> GetAsync()
        {
            return await _dbContext.AppSettings
                                   .AsNoTracking()
                                   .FirstOrDefaultAsync(s => s.Id == SettingsId);
        }

        public async Task UpsertAsync(AppSettings settings)
        {
            var exists = await _dbContext.AppSettings
                                         .AnyAsync(s => s.Id == SettingsId);

            if (exists)
            {
                _dbContext.AppSettings.Update(settings);
            }
            else
            {
                settings.Id = SettingsId;
                await _dbContext.AppSettings.AddAsync(settings);
            }

            await _dbContext.SaveChangesAsync();
        }
    }
}
