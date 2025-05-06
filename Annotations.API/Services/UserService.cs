using Annotations.Core.Models;
using Microsoft.EntityFrameworkCore;


namespace Annotations.API.Services;


/// <summary>
/// Defines a service for accessing user data.
/// </summary>
public interface IUserService
{
    Task<List<AdminUserModel>> GetAdmins();

    Task<List<MedicalProfessionalUserModel>> GetMedicalProfessionals();
}



public class UserService : IUserService
{
    private AnnotationsDbContext _context;



    /// <summary>
    /// Constructor of the service class. 
    /// </summary>
    /// <param name="context"> Annotations database context containing the user data. </param>
    public UserService(AnnotationsDbContext context)
    {
        _context = context;
    }

    
    
    /// <summary>
    /// Retrieves all admins. 
    /// </summary>
    /// <returns> A list of all AdminUserModel. </returns>
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

    
    
    /// <summary>
    /// Retrieves all medical professionals. 
    /// </summary>
    /// <returns> A list of all MedicalProfessionalUserModel. </returns>
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
