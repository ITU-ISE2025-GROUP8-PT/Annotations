using Microsoft.AspNetCore.Mvc;

namespace Annotations.API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class UserDataController : ControllerBase
    {
        private static readonly string[] UserNames =
        [
            "Sophie", "Ida", "Theresia", "Jakob", "Jacob ", "Ronas", "Nickie", "Radmehr", "Giorgi", "Daniel"
        ];
        
        private static readonly string[] Occupation =
        [
            "Medical Professional", "Administrator"
        ];

        private readonly ILogger<UserDataController> _logger;

        public UserDataController(ILogger<UserDataController> logger)
        {
            _logger = logger;
        }

        [HttpGet]
        public IEnumerable<UserData> Get(int days = 7)
        {
            return Enumerable.Range(1, days).Select(index => new UserData
                (
                    UserNames[Random.Shared.Next(UserNames.Length)],
                    Random.Shared.Next(1, 55),
                    DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
                    Occupation[Random.Shared.Next(Occupation.Length)]
                ))
                .ToArray();
        }
    }
}