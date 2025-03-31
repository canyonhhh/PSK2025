using Microsoft.AspNetCore.Mvc;
using PSK2025.ApiService.Services.Interfaces;

namespace PSK2025.ApiService.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class RandomNumberController : ControllerBase
    {
        private readonly IRandomNumberService _randomNumberService;

        public RandomNumberController(IRandomNumberService randomNumberService)
        {
            _randomNumberService = randomNumberService;
        }

        [HttpGet]
        public IActionResult GetRandomNumber()
        {
            var randomNumber = _randomNumberService.GenerateRandomNumber();
            return Ok(new { number = randomNumber });
        }
    }
}