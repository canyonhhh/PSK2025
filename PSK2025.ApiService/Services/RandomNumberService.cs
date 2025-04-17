using PSK2025.ApiService.Services.Interfaces;

namespace PSK2025.ApiService.Services
{
    public class RandomNumberService : IRandomNumberService
    {
        public int GenerateRandomNumber()
        {
            Random random = new Random();
            return random.Next(100_000_000, 999_999_999);
        }

    }
}