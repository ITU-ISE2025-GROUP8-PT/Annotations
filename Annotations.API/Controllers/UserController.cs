using Annotations.Core.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Annotations.API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class UserController : ControllerBase
    {
        private readonly AnnotationsDbContext _context;
        private readonly ILogger<UserController> _logger;

        public UserController(AnnotationsDbContext context, ILogger<UserController> logger)
        {
            _context = context;
            _logger = logger;
        }

        [HttpGet("admins")]
        public async Task<ActionResult<IEnumerable<AdminUserModel>>> GetAdmins()
        {
            // Fetch users from the database
            var admins = await _context.Admins
                .Select(u => new AdminUserModel
                {
                    Id = u.UserId,
                    FirstName = u.FirstName,
                    LastName = u.LastName,
                    Email = u.Email
                })
                .ToListAsync();
            
            return Ok(admins);
        }
        
        [HttpGet("mps")]
        public async Task<ActionResult<IEnumerable<MedicalProfessionalUserModel>>> GetMedicalProfessionals()
        {
            var medicalProfessionals = await _context.MedicalProfessionals
                .Select(u => new MedicalProfessionalUserModel
                {
                    Id = u.UserId,
                    FirstName = u.FirstName,
                    LastName = u.LastName,
                    Email = u.Email,
                    Affiliation = u.Affiliation,
                    JobTitle = u.JobTitle,
                    TotalAssignmentsFinished = u.TotalAssignmentsFinished,
                    ProfilePictureID = u.ProfilePictureId
                })
                .ToListAsync();
            
            return Ok(medicalProfessionals);
        }
    }
}
