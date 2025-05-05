using Annotations.API.Services;
using Annotations.Core.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Annotations.API.Groups;

public static class UserEndpoints

{
    /// <summary>
    /// Where all the endpoints are initialized to their respective handler
    /// </summary>
    /// <param name="pathBuilder"></param>
    /// <exception cref="InvalidOperationException"></exception>
    public static void MapEndpoints(RouteGroupBuilder pathBuilder)
    {
        pathBuilder.RequireAuthorization().DisableAntiforgery();
        pathBuilder.MapGet("/admins", RetrieveAdmins);
        pathBuilder.MapGet("/medicalprofessionals", RetrieveMedicalProfessionals);
        pathBuilder.MapGet("/exception", () =>
        {
            throw new InvalidOperationException(
                "Exception has been raised in the API. Look for further details in the log.");
        });
    }

    
    
    /// <summary>
    /// Retrieves all registered admins 
    /// </summary>
    /// <param name="context">The DbContext where the admins are located</param>
    /// <param name="_userService">The injected service class with methods</param>
    /// <returns>A list of the AdminUserModels</returns>
    public static async Task<List<AdminUserModel>> RetrieveAdmins(
        AnnotationsDbContext context, [FromServices] IUserService _userService)
    {
        return await _userService.GetAdmins();
    }

    
    
    /// <summary>
    /// Retrieves all registered medical professionals 
    /// </summary>
    /// <param name="context">The DbContext where the medical professionals  are located</param>
    /// <param name="_userService">The the injected service class with methods</param>
    /// <returns>A list of the MedicalProfessionalUserModels</returns>
    public static async Task<List<MedicalProfessionalUserModel>> RetrieveMedicalProfessionals(
        AnnotationsDbContext context, [FromServices] IUserService _userService)
    {
        return await _userService.GetMedicalProfessionals();
    }
}
