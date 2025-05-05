using Annotations.Core.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Annotations.API.Groups;

public static class UserEndpoints
{
    public static void MapEndpoints(RouteGroupBuilder pathBuilder)
    {
        pathBuilder.RequireAuthorization().DisableAntiforgery();
        pathBuilder.MapGet("/", () => "Hello Kitty!");
        pathBuilder.MapGet("/admins", RetrieveAdmins);
        pathBuilder.MapGet("/medicalprofessionals", RetrieveMedicalProfessionals);
        pathBuilder.MapGet("/exception", () => 
        {
            throw new InvalidOperationException("Exception has been raised in the API. Look for further details in the log.");
        });

        
    }
    public static async Task<List<AdminUserModel>> RetrieveAdmins(
        AnnotationsDbContext context)
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
    }

    public static async Task<List<MedicalProfessionalUserModel>> RetrieveMedicalProfessionals(
        AnnotationsDbContext context)
    {
        var medicalProfessionals = await context.MedicalProfessionals
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
