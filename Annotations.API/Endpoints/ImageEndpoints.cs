using System.Security.Claims;
using Annotations.API.Services.Datasets;
using Annotations.API.Services.Images;
using Annotations.API.Services.Users;
using Annotations.Core.Models;
using Microsoft.AspNetCore.Mvc;


namespace Annotations.API.Endpoints;


/// <summary>
/// Endpoints for images, and image metadata.
/// </summary>
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

        pathBuilder.MapGet("/get-metadata/{imageId}", GetMetaDataHandler);

        pathBuilder.MapGet("/filter/{category}", FilterImagesHandler);
    }





    /// <summary>
    /// This is the upload endpoint where the image first gets validated. 
    /// The image's metadata is uploaded into a SQLite database.
    /// The image itself is uploaded into Azure Blob Storage. 
    /// </summary>
    /// <param name="image"> The file containing the image. </param>
    /// <param name="category"> Which category the image should have. </param>
    /// <param name="claimsPrincipal">For authorization</param>
    /// <param name="httpContext"></param>
    /// <param name="uploader">Where the uploading takes place</param>
    /// <param name="userService">Used to assign a user to the image</param>
    /// <returns></returns>
    private static async Task<ImageUploaderResult> UploadImageHandler(
        IFormFile       image, 
        string          category,
        ClaimsPrincipal claimsPrincipal,
        HttpContext     httpContext,
        [FromServices] IImageUploader uploader,
        [FromServices] IUserService userService)
    {
        var user = await userService.TryFindUserAsync(claimsPrincipal) ?? await userService.CreateUserAsync(claimsPrincipal);

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
        [FromRoute] int imageId, 
        [FromServices] IImageService imageService)
    {
        return Results.StatusCode((int) await imageService.DeleteImageAsync(imageId));
    }





    /// <summary>
    /// Retrieves an image based on the provided imageid.
    /// </summary>
    /// <param name="imageId"></param>
    /// <param name="httpContext"></param>
    /// <param name="imageService"> An image service instance. </param>
    /// <returns> Image downloadable as stream. </returns>
    private static async Task<IResult> GetImageHandler(
        [FromRoute] int imageId,
        HttpContext httpContext,
        [FromServices] IImageService imageService)
    {    
        var getImageResult = await imageService.GetImageAsync(imageId);

        httpContext.Response.StatusCode = getImageResult.StatusCode;
        return Results.Stream(getImageResult.Stream, contentType: getImageResult.ContentType);
    }





    /// <summary>
    /// Retrieves the metadata of an image based on the provided imageid.
    /// </summary>
    /// <param name="imageId"></param>
    /// <param name="imageService"></param>
    /// <returns></returns>
    private static async Task<IResult> GetMetaDataHandler(
        [FromRoute] int imageId,
        [FromServices] IImageService imageService)
    {
        var imageModel = await imageService.GetMetadataAsync(imageId);
        return imageModel != null
            ? Results.Ok(imageModel)
            : Results.NotFound();
    }





    /// <summary>
    /// Finds and returns all images within a certain category.
    /// </summary>
    /// <param name="category"></param>
    /// <param name="imageService"> An image service instance. </param>
    /// <returns> An array of images as JSON string with the wanted category. </returns>
    private static async Task<ICollection<ImageModel>> FilterImagesHandler(
        [FromRoute] string category,
        [FromServices] IImageService imageService)
    {
        return await imageService.GetImagesByCategoryAsync(category);
    }
}
