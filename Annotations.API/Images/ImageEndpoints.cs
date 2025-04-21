using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;
using Annotations.API.Users;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Net.Http.Headers;

namespace Annotations.API.Images;

public class ImageEndpoints
{
    /// <summary>
    /// Maps all application endpoints for image upload and download. 
    /// </summary>
    /// <param name="groupBuilder"></param>
    public static void MapEndpoints(RouteGroupBuilder groupBuilder)
    {
        groupBuilder.RequireAuthorization().DisableAntiforgery(); // Disabled as this is an API, which will not serve any forms.
        groupBuilder.MapPost("/Upload", UploadImageHandler);
        groupBuilder.MapGet("/Download/{imageId}", DownloadImageHandler);
    }


    /// <summary>
    /// Handler for post request to upload an image.
    /// </summary>
    static async Task<ImageUploaderResult> UploadImageHandler(
        IFormFile       image,
        long?           addToSeries, // TODO: Allow upload into existing image series directly.
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


    /// <summary>
    /// Handler for get request to download full image from storage.
    /// </summary>
    /// <param name="imageId">URI for image to download.</param>
    static async Task<IResult> DownloadImageHandler(
        [FromRoute] string imageId,
        HttpContext httpContext,
        [FromServices] IImageService imageService
        )
    {
        var downloadResult = await imageService.DownloadImageAsync(imageId);

        httpContext.Response.StatusCode = downloadResult.StatusCode;
        return Results.Stream(downloadResult.Stream, contentType: downloadResult.ContentType, fileDownloadName: imageId);
    }
}
