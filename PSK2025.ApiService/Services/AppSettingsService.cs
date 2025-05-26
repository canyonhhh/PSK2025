using PSK2025.ApiService.Services.Interfaces;
using PSK2025.Data.Repositories.Interfaces;
using PSK2025.Models.Entities;
using PSK2025.Models.Enums;
using System;
using System.Threading.Tasks;

namespace PSK2025.ApiService.Services
{
    public class AppSettingsService : IAppSettingsService
    {
        private readonly IAppSettingsRepository _repo;

        public AppSettingsService(IAppSettingsRepository repo)
        {
            _repo = repo;
        }

        public async Task<(bool Paused, ServiceError Error)> GetOrderingStatusAsync()
        {
            try
            {
                var settings = await _repo.GetAsync();
                var paused = settings?.OrderingPaused ?? false;
                return (paused, ServiceError.None);
            }
            catch
            {
                return (false, ServiceError.DatabaseError);
            }
        }

        public async Task<ServiceError> PauseOrderingAsync()
        {
            try
            {
                var settings = await _repo.GetAsync() ?? new AppSettings();
                settings.OrderingPaused = true;
                await _repo.UpsertAsync(settings);
                return ServiceError.None;
            }
            catch (Exception)
            {
                return ServiceError.DatabaseError;
            }
        }

        public async Task<ServiceError> ResumeOrderingAsync()
        {
            try
            {
                var settings = await _repo.GetAsync() ?? new AppSettings();
                settings.OrderingPaused = false;
                await _repo.UpsertAsync(settings);
                return ServiceError.None;
            }
            catch (Exception)
            {
                return ServiceError.DatabaseError;
            }
        }
    }
}
