using PSK2025.ApiService.Services.Interfaces;
using System.Security.Claims;

namespace PSK2025.ApiService.Services
{
    public class GetUserIdService : IGetUserIdService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public GetUserIdService(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public Guid GetUserIdFromToken()
        {
            var userIdClaim = _httpContextAccessor.HttpContext?.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(userIdClaim))
            {
                throw new UnauthorizedAccessException("User ID not found in token.");
            }

            return Guid.Parse(userIdClaim);
        }
    }
}
