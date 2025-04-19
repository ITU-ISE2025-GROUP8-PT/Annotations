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
        groupBuilder.MapPost("/Upload", UploadImage).RequireAuthorization();
    }

    /// <summary>
    /// Handler method for post request to upload an image.
    /// </summary>
    /// <param name="image"></param>
    /// <param name="series"></param>
    /// <param name="user"></param>
    /// <returns></returns>
    static async Task<IResult> UploadImage(
        IFormFile       image,
        long            series,
        ClaimsPrincipal user,
        [FromServices] IImageUploader uploader
        )
    {
        using MemoryStream stream = new MemoryStream();
        await image.OpenReadStream().CopyToAsync(stream);

        return Results.Created();
    }
}
