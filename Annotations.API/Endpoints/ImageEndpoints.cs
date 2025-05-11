using System.Security.Claims;
using Annotations.API.Services.Datasets;
using Annotations.API.Services.Images;
using Annotations.API.Services.Users;
using Annotations.Core.Models;
using Microsoft.AspNetCore.Mvc;


namespace Annotations.API.Endpoints;

public static class ImageEndpoints
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
        
        pathBuilder.MapPost("/upload", UploadImageHandler);
        
        pathBuilder.MapGet("/get/{imageId}", GetImageHandler);

        pathBuilder.MapDelete("/delete/{imageId}", DeleteImageHandler);

        pathBuilder.MapGet("/APITest", () =>
        {
            return new string[] { "1", "2", "3", "Dette er en prøve" };
        });
    }



    /// <summary>
    /// This is the upload endpoint where the image first gets validated. 
    /// This is then uploaded into Azure Blob Storage as a JSON file. 
    /// </summary>
    /// <param name="image"> The file containing the image. </param>
    /// <param name="category"> Which category the image should have. </param>
    /// <param name="context"> The SQLite database context. </param>
    /// <param name="_imageService"> An image service instance. </param>
    /// <returns></returns>
    private static async Task<ImageUploaderResult> UploadImageHandler(
        IFormFile       image, 
        string          category,
        ClaimsPrincipal claimsPrincipal,
        HttpContext     httpContext,
        [FromServices] IImageUploader uploader,
        [FromServices] IUserService userService)
    {
        var user = await userService.TryFindUserAsync(claimsPrincipal) ?? await userService.CreateUser(claimsPrincipal);

        uploader.OriginalFilename = image.FileName;
        uploader.ContentType      = image.ContentType;
        uploader.InputStream      = image.OpenReadStream();
        uploader.UploadedBy       = user;
        uploader.Category         = category;

        var uploaderResult = await uploader.StoreAsync();

        httpContext.Response.StatusCode = uploaderResult.StatusCode;
        return uploaderResult;
    }



    /// <summary>
    /// Deletes an image.
    /// </summary>
    /// <param name="imageId"> Image UID. </param>
    /// <param name="imageService"> An image service instance. </param>
    /// <returns> The status code of deleting the image. </returns>
    private static async Task<IResult> DeleteImageHandler(
        [FromRoute] string imageId, 
        [FromServices] IImageService imageService)
    {
        return Results.StatusCode((int) await imageService.DeleteImageAsync(imageId));
    }



    /// <summary>
    /// Retrieves an image based on the provided imageid.
    /// </summary>
    /// <param name="imageId"></param>
    /// <param name="imageService"> An image service instance. </param>
    /// <returns> Image downloadable as stream. </returns>
    private static async Task<IResult> GetImageHandler(
        [FromRoute] string imageId,
        HttpContext httpContext,
        [FromServices] IImageService imageService)
    {    
        var getImageResult = await imageService.GetImageAsync(imageId);

        httpContext.Response.StatusCode = getImageResult.StatusCode;
        return Results.Stream(getImageResult.Stream, contentType: getImageResult.ContentType);
    }
}
