using Microsoft.AspNetCore.Http;
using System.Security.Claims;
namespace PSK2025.ApiService.Services.Interfaces
{
    public interface IGetUserIdService
    {
        string GetUserIdFromToken();
    }
}