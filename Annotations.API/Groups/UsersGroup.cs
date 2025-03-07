using Annotations.Core.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Annotations.API.Groups;

public static class UsersGroup
{
    public static void MapEndpoints(RouteGroupBuilder pathBuilder)
    {
        pathBuilder.MapGet("/", () => "Hello Kitty!");
        pathBuilder.MapGet("/GetAdmins", async (AnnotationsDbContext context) =>
            {
                var admins = await context.Admins
                    .Select(u => new AdminUserModel
                    {
                        Id = u.UserId,
                        FirstName = u.FirstName,
                        LastName = u.LastName,
                        Email = u.Email
                    })
                    .ToListAsync();
                return admins;
            });
        pathBuilder.MapGet("/exception", () => 
        {
            throw new InvalidOperationException("Exception has been raised in the API. Look for further details in the log.");
        });
    }
}


/*
{
    [ApiController]
    [Route("[controller]")]
    public class UsersGroup : ControllerBase
    {
        private readonly AnnotationsDbContext _context;
        private readonly ILogger<UsersGroup> _logger;

        public UsersGroup(AnnotationsDbContext context, ILogger<UsersGroup> logger)
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
*/
