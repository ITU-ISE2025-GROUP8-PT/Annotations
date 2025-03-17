using Annotations.Core.Entities;
using Annotations.Core.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Annotations.API.Groups;

public static class AuthGroup
{
    public static void MapEndpoints(RouteGroupBuilder pathBuilder)
    {
        pathBuilder.MapPost("/SignIn", async ([FromBody] SignInRequest request, [FromServices] SignInManager<AnnotationsUser> signInManager) => 
        {
            var result = await signInManager.PasswordSignInAsync(request.Username, request.Password, true, false);

            if (!result.Succeeded) return Results.BadRequest("Access denied.");

            var user = await signInManager.UserManager.FindByNameAsync(request.Username) ?? throw new ArgumentException("Login successful, but user not found.");
            var claims = await signInManager.CreateUserPrincipalAsync(user);

            // TODO: Create Bearer token return.
            return Results.Accepted("Access Granted!");
        });
    }
}
