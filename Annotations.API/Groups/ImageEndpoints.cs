using Annotations.API.Services;
using Annotations.Core.Models;
using Microsoft.AspNetCore.Http.Extensions;
using Annotations.Core.Entities;
using Azure;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.Extensions.Azure;
using SQLitePCL;


namespace Annotations.API.Groups;

public static class ImageEndpoints
{
    private static int _counter = 0;//change this - it gets reset every time the program resets
    
 
    public static void MapEndpoints(RouteGroupBuilder pathBuilder)
    {
        pathBuilder.RequireAuthorization().DisableAntiforgery();
        
        pathBuilder.MapPost("/upload", UploadImageHandler);
        
        pathBuilder.MapGet("/{imageId}", RetrieveImageHandler);

        pathBuilder.MapDelete("/{imageId}", DeleteImageHandler);

        pathBuilder.MapGet("/filter/{category}", FilterImagesHandler);

        pathBuilder.MapGet("/datasets", RetrieveDatasetHandler);

        pathBuilder.MapGet("/datasets/{dataset}", RetrieveImagesFromDatasetHandler);
        
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

    
    //This is the upload endpoint where the image first gets validated,
    //and then gets uploaded into your local Azurite BlobStorage as a JSON file

    public static async Task<IResult> UploadImageHandler(IFormFile image, string category, AnnotationsDbContext context, 
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

    public static async Task<IResult> DeleteImageHandler(string imageId, [FromServices] IAzureClientFactory<BlobServiceClient> clientFactory, 
        [FromServices] IImageService _imageService)
    {
        var cts = new CancellationTokenSource(5000);

        var containerClient = _imageService.createContainer();
        BlobClient blobClient = containerClient.GetBlobClient(imageId + ".json");
        if (!blobClient.Exists(cts.Token).ToString().Contains("404"))
        {
            /*A snapshot is a read-only version of a blob that's taken at a point in time.
            As of right now, we do not make snapshots of blobs, but it is still possible to manually create.*/
            await blobClient.DeleteAsync(snapshotsOption: DeleteSnapshotsOption.IncludeSnapshots);
            Console.WriteLine("image deleted successfully");
            return Results.StatusCode(204);
        }

            
        Console.WriteLine("Cannot delete image because it doesn't exist");
        return Results.StatusCode(404);
    }
    
    public static async Task<string> RetrieveImageHandler([FromRoute] string imageId, [FromServices] IAzureClientFactory<BlobServiceClient> clientFactory, 
        [FromServices] IImageService _imageService)
    {
        //insert password restrictions here 🐿️
        var cts = new CancellationTokenSource(5000);
        var containerClient = _imageService.createContainer();

        var getImageResult = await _imageService.GetImage(imageId, cts,containerClient);

        if (getImageResult.Success)
        {
            return getImageResult.image; 
        }
        
        //will only reach here if it cannot find an image with the id of the correct file type,
        //or else the request will terminate inside the for-loop
        Console.WriteLine("Cannot retrieve image because it doesn't exist");
        return "Cannot retrieve image because it doesn't exist";
    }
    
    public static async Task<string[]> FilterImagesHandler(string category, [FromServices] IAzureClientFactory<BlobServiceClient> clientFactory, 
        [FromServices] IImageService _imageService)
    {
        var cts = new CancellationTokenSource(5000);

        HashSet<string> collection = await _imageService.Filter(category);

        if (collection.Count() == 0)
        {
            Console.WriteLine("No pictures have this category");
            //TODO return proper error code
            //return Results.StatusCode(404);
        }
        
        return collection.ToArray();//returns array of the JSON files as strings

    }
    
    public static async Task<DatasetModel[]> RetrieveDatasetHandler(AnnotationsDbContext context, [FromServices] IImageService _imageService)
    {
        var cts = new CancellationTokenSource(5000);

        
        return await _imageService.GetAllDatasets();

    }
    
    public static async Task<string[]> RetrieveImagesFromDatasetHandler(string dataset, AnnotationsDbContext context, 
        [FromServices] IAzureClientFactory<BlobServiceClient> clientFactory, [FromServices] IImageService _imageService)
    {
         //TODO: almost identical code as "/filter/{category}" - remove the code duplication
            var cts = new CancellationTokenSource(5000);

            var datasetModel = await _imageService.GetDataset(dataset);
            
            HashSet<string> collection = new HashSet<string>();
            var containerClient = _imageService.createContainer();

            //foreach (DatasetModel datasetModel in datasets)//guaranteed to only be one dataset in "datasets", so this is not linear time. 
            //{//there is a better way of doing this
            
                foreach (int ids in datasetModel.ImageIds)
                {
                    Console.WriteLine(ids);
                    var getImageResult = await _imageService.GetImage(ids.ToString(),cts, containerClient);
                    if (getImageResult.Success)
                    {
                        collection.Add(getImageResult.image);
                    }
                    else
                    {
                        Console.WriteLine("Cannot retrieve image because it doesn't exist");
                        break;
                    }
                }
            //}
            return collection.ToArray();//returns array of all images as JSON strings
            
    }
}