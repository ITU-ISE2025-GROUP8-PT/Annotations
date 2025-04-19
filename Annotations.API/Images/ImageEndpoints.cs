using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;

namespace Annotations.API.Images;

public class ImageEndpoints
{
    /// <summary>
    /// Maps all application endpoints for image upload and download. 
    /// </summary>
    /// <param name="groupBuilder"></param>
    public static void MapEndpoints(RouteGroupBuilder groupBuilder)
    {
        groupBuilder.DisableAntiforgery(); // Disabled as this is an API, which will not serve any forms.
        groupBuilder.MapPost("/upload", ImagesUploadDelegate).RequireAuthorization();
    }

    /// <summary>
    /// <para>Delegate for <c>/images/upload</c> POST request.</para>
    /// </summary>
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
