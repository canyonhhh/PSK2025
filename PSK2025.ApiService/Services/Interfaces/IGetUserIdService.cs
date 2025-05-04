using System.Security.Claims;
using Microsoft.AspNetCore.Http;
namespace PSK2025.ApiService.Services.Interfaces
{
    public interface IGetUserIdService
    {
        Guid GetUserIdFromToken();
    }
}
