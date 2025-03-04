using Annotations.API.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Annotations.API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class DbUserController : ControllerBase
    {
        private readonly AnnotationsDbContext _context;
        private readonly ILogger<UserDataController> _logger;

        public DbUserController(AnnotationsDbContext context, ILogger<UserDataController> logger)
        {
            _context = context;
            _logger = logger;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<UserDto>>> Get()
        {
            // Fetch users from the database
            var users = await _context.Userz
                .Select(u => new UserDto
                {
                    Id = u.UserId,
                    FirstName = u.FirstName,
                    LastName = u.LastName,
                    Email = u.Email
                })
                .ToListAsync();
            
            return Ok(users);
        }
    }
}