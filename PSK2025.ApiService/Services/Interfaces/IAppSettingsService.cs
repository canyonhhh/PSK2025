using PSK2025.Models.Enums;
using System.Threading.Tasks;

namespace PSK2025.ApiService.Services.Interfaces
{
    public interface IAppSettingsService
    {
        /// <summary>
        /// Gražina, ar ordering paused, arba klaidą.
        /// </summary>
        Task<(bool Paused, ServiceError Error)> GetOrderingStatusAsync();

        Task<ServiceError> PauseOrderingAsync();
        Task<ServiceError> ResumeOrderingAsync();
    }
}
