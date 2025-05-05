using Annotations.Core.Entities;
using Annotations.Core.Models;
using Microsoft.EntityFrameworkCore;

namespace Annotations.API.Services;

public interface IUserService
{
    Task<List<AdminUserModel>> GetAdmins();
    Task<List<MedicalProfessionalUserModel>> GetMedicalProfessionals();
}
public class UserService : IUserService
{
    private AnnotationsDbContext _context;
    public UserService(AnnotationsDbContext context)
    {
        _context = context;
    }

    public async Task<List<AdminUserModel>> GetAdmins()
    {
        var admins = await _context.Admins
            .Select(u => new AdminUserModel
            {
                Id = u.UserId,
                FirstName = u.FirstName,
                LastName = u.LastName,
                Email = u.Email
            })
            .ToListAsync();
        return admins;
    }

    public async Task<List<MedicalProfessionalUserModel>> GetMedicalProfessionals()
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
        return medicalProfessionals;
    }
}