using PSK2025.Models.Entities;
using System.Threading.Tasks;

namespace PSK2025.Data.Repositories.Interfaces
{
    public interface IAppSettingsRepository
    {
        Task<AppSettings?> GetAsync();
        Task UpsertAsync(AppSettings settings);
    }
}