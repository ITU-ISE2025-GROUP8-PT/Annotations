using Annotations.API.Services;
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
        AnnotationsDbContext context, [FromServices] IUserService _userService)
    {
        return await _userService.GetAdmins();
    }

    public static async Task<List<MedicalProfessionalUserModel>> RetrieveMedicalProfessionals(
        AnnotationsDbContext context, [FromServices] IUserService _userService)
    {
        return await _userService.GetMedicalProfessionals();
    }
}
