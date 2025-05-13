using Annotations.API.Services.Users;
using Annotations.Core.Models;
using Microsoft.AspNetCore.Mvc;


namespace Annotations.API.Endpoints;

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
        
        pathBuilder.MapGet("/users", GetAllUsers);
    }



    /// <summary>
    /// Retrieves all users. 
    /// </summary>
    /// <param name="context"> Annotations database context containing the user data. </param>
    /// <param name="_userService"> A user service instance. </param>
    /// <returns> A list of the AdminUserModels. </returns>
    public static async Task<List<UserModel>> GetAllUsers(
        AnnotationsDbContext context,
        [FromServices] IUserService _userService)
    {
        return await _userService.GetUsersAsync();
    }
}
