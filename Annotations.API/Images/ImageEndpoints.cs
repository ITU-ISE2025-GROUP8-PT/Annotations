using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;

namespace Annotations.API.Images;

public class ImageEndpoints
{
    public static void MapEndpoints(RouteGroupBuilder groupBuilder)
    {
        groupBuilder.DisableAntiforgery();
        groupBuilder.MapPost("/upload", ImagesUploadDelegate).RequireAuthorization();
    }

    private static Delegate ImagesUploadDelegate => async (
        IFormFile       image,
        string          series,
        ClaimsPrincipal user) =>
    {
        if (user == null)
        {
            // Deny anonymous.
            return Results.Forbid();
        }
        
        using MemoryStream stream = new MemoryStream();
        await image.OpenReadStream().CopyToAsync(stream);

        return Results.Created();
    };
}
