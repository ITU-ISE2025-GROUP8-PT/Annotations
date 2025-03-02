using Microsoft.AspNetCore.Mvc;

namespace Annotations.API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class KageController : ControllerBase
    {
        private static readonly string[] Summaries =
        [
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        ];

        private readonly ILogger<KageController> _logger;

        public KageController(ILogger<KageController> logger)
        {
            _logger = logger;
        }

        [HttpGet]
        [Route("Kagevejr")]
        public IEnumerable<WeatherForecast> Get(int days = 8)
        {
            return Enumerable.Range(1, days).Select(index => new WeatherForecast
                (
                    DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
                    Random.Shared.Next(-20, 55),
                    Summaries[Random.Shared.Next(Summaries.Length)]
                ))
                .ToArray();
        }
    }
}