using Annotations.API.Services;
using Annotations.Core.Models;
using Microsoft.AspNetCore.Mvc;


namespace Annotations.API.Groups;

public static class UserEndpoints
{
    /// <summary>
    /// Where all the endpoints are initialized to their respective handler. 
    /// </summary>
    /// <param name="pathBuilder"></param>
    public static void MapEndpoints(RouteGroupBuilder pathBuilder)
    {
        // Anti-forgery is disabled. This was decided because the backend will not serve any forms.
        // Anti-forgery measures are covered in the front-end, and by the JWT token protection. 
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
    /// Retrieves all registered admins. 
    /// </summary>
    /// <param name="context"> Annotations database context containing the user data. </param>
    /// <param name="_userService"> A user service instance. </param>
    /// <returns> A list of the AdminUserModels. </returns>
    public static async Task<List<AdminUserModel>> RetrieveAdmins(
        AnnotationsDbContext context, 
        [FromServices] IUserService _userService)
    {
        return await _userService.GetAdmins();
    }



    /// <summary>
    /// Retrieves all registered medical professionals. 
    /// </summary>
    /// <param name="context"> Annotations database context containing the user data. </param>
    /// <param name="_userService"> A user service instance. </param>
    /// <returns> A list of the MedicalProfessionalUserModels. </returns>
    public static async Task<List<MedicalProfessionalUserModel>> RetrieveMedicalProfessionals(
        AnnotationsDbContext context, 
        [FromServices] IUserService _userService)
    {
        return await _userService.GetMedicalProfessionals();
    }
}
