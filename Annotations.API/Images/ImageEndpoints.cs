using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;
using Annotations.API.Users;
using System.Text;
using System.Text.Json;

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
        groupBuilder.MapPost("/Upload", UploadImageHandler).RequireAuthorization();
    }

    /// <summary>
    /// Handler method for post request to upload an image.
    /// </summary>
    static async Task<ImageUploaderResult> UploadImageHandler(
        IFormFile       image,
        long?           addToSeries,
        ClaimsPrincipal claimsPrincipal,
        HttpContext     httpContext,
        [FromServices] IImageUploader uploader,
        [FromServices] IUserService   userService
        )
    {
        var user = await userService.TryFindUserAsync(claimsPrincipal) ?? await userService.CreateUser(claimsPrincipal);

        uploader.OriginalFilename = image.FileName;
        uploader.ContentType      = image.ContentType;
        uploader.InputStream      = image.OpenReadStream();
        uploader.UploadedBy       = user;

        var uploaderResult = await uploader.StoreAsync();

        httpContext.Response.StatusCode = uploaderResult.StatusCode;

        return uploaderResult;
    }
}
