using PSK2025.Models.Entities;
using System.Threading.Tasks;

namespace PSK2025.Data.Repositories.Interfaces
{
    public interface IAppSettingsRepository
    {
        /// <summary>
        /// Gražina AppSettings eilutę (vienetuką) arba null.
        /// </summary>
        Task<AppSettings?> GetAsync();

        /// <summary>
        /// Įrašo arba atnaujina AppSettings.
        /// </summary>
        Task UpsertAsync(AppSettings settings);
    }
}
