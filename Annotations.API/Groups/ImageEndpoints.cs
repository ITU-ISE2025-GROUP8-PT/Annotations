using Annotations.API.Services;
using Annotations.Core.Models;
using Microsoft.AspNetCore.Mvc;
using Azure.Storage.Blobs;
using Microsoft.Extensions.Azure;


namespace Annotations.API.Groups;

public static class ImageEndpoints
{
    //the counter represents which id an image will get. This is reset every time the program resets
    private static int _counter = 0;
 
    
    
    /// <summary>
    /// Where all the endpoints are initialized to their respective handler
    /// </summary>
    /// <param name="pathBuilder"></param>
    /// <exception cref="InvalidOperationException"></exception>
    public static void MapEndpoints(RouteGroupBuilder pathBuilder)
    {
        pathBuilder.RequireAuthorization().DisableAntiforgery();
        
        pathBuilder.MapPost("/upload", UploadImageHandler);
        
        pathBuilder.MapGet("/{imageId}", RetrieveImageHandler);

        pathBuilder.MapDelete("/{imageId}", DeleteImageHandler);

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
    /// This is the upload endpoint where the image first gets validated,
    /// and then gets uploaded into your local Azurite BlobStorage as a JSON file
    /// </summary>
    /// <param name="image">The file containing the image</param>
    /// <param name="category"> Which category the image should have</param>
    /// <param name="context"> The SQLite database conetx</param>
    /// <param name="_imageService"> The injected service class</param>
    /// <returns></returns>
    private static async Task<IResult> UploadImageHandler(IFormFile image, string category, AnnotationsDbContext context, 
         [FromServices] IImageService _imageService)
    {
        ValidationResponse response = _imageService.ValidateImage(image);
        if (!response.Success)
        {
            _imageService.UploadImageError(response);
            return Results.StatusCode(422);
            //TODO: how should the end user see this?
        } 
        await _imageService.UploadingImage(image,_counter, category);
        _counter++;
        return Results.StatusCode(200);
    }

    
    
    /// <summary>
    /// Deletes image
    /// </summary>
    /// <param name="imageId"></param>
    /// <param name="_imageService">the injected service class with methods</param>
    /// <returns> the statuscode of deleting the image</returns>
    private static async Task<IResult> DeleteImageHandler(string imageId, [FromServices] IImageService _imageService)
    {
        bool deleting = await _imageService.DeleteImage(imageId);
        if (deleting)
        {
            return Results.StatusCode(204);
        }
            
        Console.WriteLine("Cannot delete image because it doesn't exist");
        return Results.StatusCode(404);
    }
    
    
    
    /// <summary>
    /// Retrieves an image based on the provided imageid
    /// </summary>
    /// <param name="imageId"></param>
    /// <param name="_imageService">the injected service class with methods</param>
    /// <returns> Either JSON string (if image exist) otherwise an errormessage</returns>
    private static async Task<string> RetrieveImageHandler([FromRoute] string imageId, 
        [FromServices] IImageService _imageService)
    {
        //insert password restrictions here 🐿️
        
        var getImageResult = await _imageService.GetImage(imageId);

        if (getImageResult.Success)
        {
            return getImageResult.image; 
        }
        
        Console.WriteLine("Cannot retrieve image because it doesn't exist");
        return "Cannot retrieve image because it doesn't exist";
    }
    
    
    /// <summary>
    /// finds and returns all images within a certain category
    /// </summary>
    /// <param name="category"></param>
    /// <param name="_imageService">the injected service class with methods</param>
    /// <returns> an array of images as JSON string with the wanted category</returns>
    private static async Task<string[]> FilterImagesHandler(string category, 
        [FromServices] IImageService _imageService)
    {

        HashSet<string> collection = await _imageService.Filter(category);

        if (collection.Count() == 0)
        {
            Console.WriteLine("No pictures have this category");
            //TODO return proper error code
            //return Results.StatusCode(404);
        }
        
        return collection.ToArray();
    }
    
    
    
    /// <summary>
    /// Retrieves all existing datasets 
    /// </summary>
    /// <param name="context">the DbContext where the datasets are</param>
    /// <param name="_imageService">the injected service class with methods</param>
    /// <returns>An array of all existing datasetModels</returns>
    private static async Task<DatasetModel[]> RetrieveAllDatasetHandler(AnnotationsDbContext context, 
        [FromServices] IImageService _imageService)
    {
        return await _imageService.GetAllDatasets();

    }
    
    
    
    /// <summary>
    /// Retrieves all the images inside a specific dataset
    /// </summary>
    /// <param name="datasetId">ID of the desired dataset</param>
    /// <param name="context">DbContext where the dataset is located</param>
    /// <param name="_imageService">the injected service class with methods</param>
    /// <returns>A string array of the needed images as a JSON string</returns>
    private static async Task<string[]> RetrieveImagesFromDatasetHandler(string datasetId, AnnotationsDbContext context, 
        [FromServices] IImageService _imageService)
    {
         //TODO: almost identical code as "/filter/{category}" - remove the code duplication

            var datasetModel = await _imageService.GetDataset(datasetId);
            
            HashSet<string> collection = new HashSet<string>();

            //guaranteed to only be one dataset in "datasets", so this is constant time. 
            //there is a better way of doing this
            
                foreach (int ids in datasetModel.ImageIds)
                {
                    Console.WriteLine(ids);
                    var getImageResult = await _imageService.GetImage(ids.ToString());
                    if (getImageResult.Success)
                    {
                        collection.Add(getImageResult.image);
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