using System.Security.Claims;
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

        pathBuilder.MapGet("/filter/{category}", FilterImagesHandler);

        pathBuilder.MapGet("/datasets", RetrieveAllDatasetHandler);

        pathBuilder.MapGet("/datasets/{datasetId}", RetrieveImagesFromDatasetHandler);
        
        pathBuilder.MapGet("/exception",
            () =>
            {
                throw new InvalidOperationException(
                    "Exception has been raised in the API. Look for further details in the log.");
            });

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
        string?         category,
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


    /// <summary>
    /// Finds and returns all images within a certain category.
    /// </summary>
    /// <param name="category"></param>
    /// <param name="imageService"> An image service instance. </param>
    /// <returns> An array of images as JSON string with the wanted category. </returns>
    private static async Task<string[]> FilterImagesHandler(
        string category, 
        [FromServices] IImageService imageService)
    {

        HashSet<string> collection = await imageService.Filter(category);

        if (collection.Count() == 0)
        {
            Console.WriteLine("No pictures have this category");
            //TODO return proper error code
            //return Results.StatusCode(404);
        }
        
        return collection.ToArray();
    }



    /// <summary>
    /// Retrieves all existing datasets. 
    /// </summary>
    /// <param name="context"> Annotations database context containing the datasets. </param>
    /// <param name="imageService"> An image service instance. </param>
    /// <returns> An array of all existing DatasetModels. </returns>
    private static async Task<DatasetModel[]> RetrieveAllDatasetHandler(
        AnnotationsDbContext context, 
        [FromServices] IImageService imageService)
    {
        return await imageService.GetAllDatasets();

    }



    /// <summary>
    /// Retrieves all the images inside a specific dataset. 
    /// </summary>
    /// <param name="datasetId"> ID of the desired dataset. </param>
    /// <param name="context"> Annotations database context containing the datasets. </param>
    /// <param name="imageService"> An image service instance. </param>
    /// <returns> A string array of the needed images as a JSON string. </returns>
    private static async Task<string[]> RetrieveImagesFromDatasetHandler(
        string datasetId, 
        AnnotationsDbContext context, 
        [FromServices] IImageService imageService)
    {
         //TODO: almost identical code as "/filter/{category}" - remove the code duplication

            var datasetModel = await imageService.GetDataset(datasetId);
            
            HashSet<string> collection = new HashSet<string>();

            //guaranteed to only be one dataset in "datasets", so this is constant time. 
            //there is a better way of doing this
            
                foreach (int ids in datasetModel.ImageIds)
                {
                    Console.WriteLine(ids);
                    var getImageResult = await imageService.GetImageAsync(ids.ToString());
                    if (getImageResult.StatusCode == 200)
                    {
                        collection.Add($"Image ID: {ids}");
                    }
                    else
                    {
                        Console.WriteLine("Cannot retrieve image because it doesn't exist");
                        //needed proper error handling here
                        //an actual error should be returned (e.g. status code 404)
                        break;
                    }
                }
            
            return collection.ToArray();
            
    }
}
