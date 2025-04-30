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
        pathBuilder.RequireAuthorization();
        
        //This is the upload endpoint where the image first gets validated, and then gets uploaded into your local Azurite BlobStorage as a JSON file
        pathBuilder.MapPost("/upload",
            async (IFormFile image, string category, AnnotationsDbContext context, [FromServices] IAzureClientFactory<BlobServiceClient> clientFactory, [FromServices] IImageService _imageService) =>
            {//TODO: more paramters for the other fields in images?

                ValidationResponse response = _imageService.ValidateImage(image);
                if (!response.Success)
                {
                    Console.WriteLine("rejecting image");
                    Console.WriteLine(response.Message);
                    return Results.StatusCode(422);
                    //TODO: how should the end user see this?
                }
                else
                {
                    var containerClient = _imageService.createContainer(clientFactory);


                    //fileExtension will always be a proper fileExtension because of the ValidateImage method


                    using (MemoryStream ms = new MemoryStream())
                    {
                        await image.OpenReadStream().CopyToAsync(ms);
                        var thisImage = new ImageModel()//TODO: dont do this - the title, description and datasetsId are hardcoded
                        {
                            Id = _counter,
                            Title = "idk",
                            Description = "description",
                            ImageData = ms.ToArray(),
                            Category = category,
                            DatasetsIds = new List<int>(){1, 2},
                        };
                        /*
                        Below functionality of cross-adding the image to the assigned datasets
                        is overriden by the hard-code creation of datasets in Api, Program.cs
                        The idea for future reference is that when uploading an image, the user
                        inputs the names (ids) of datasets, and we can use below code (without
                        hard-coded ids) to cross-add the images to the relevant datasets.
                        */
                        var neededDataset = context.Datasets.Select(Dataset => Dataset).
                        Where(Dataset => Dataset.Id == 1 || Dataset.Id == 2);//TODO dont do this - this is hardcoded for testing
                        
                        foreach (Dataset dataset in neededDataset)
                            
                        {
                            dataset.ImageIds.Add(thisImage.Id);//adds images to the datasets
                            await context.SaveChangesAsync();
                        }

                        string jsonString = System.Text.Json.JsonSerializer.Serialize(thisImage);//objects becomes JSON string
                        var byteContent = System.Text.Encoding.UTF8.GetBytes(jsonString);//JSON string becomes byte array

                        
                        BlobClient thisImageBlobClient = containerClient.GetBlobClient($"{_counter}.json");
                        _counter++;

                        var blobHeaders = new BlobHttpHeaders
                        {
                            ContentType = "application/json"
                        };

                        // Trigger the upload function to push the data to blob
                        await thisImageBlobClient.UploadAsync(new MemoryStream(byteContent), blobHeaders);//uploaded as byte array
                        //Should it be uploaded as a string instead? So far all endpoints retrieve the JSON files as strings?
                    }
                   
                    

                    return Results.StatusCode(200);
                }


            }).DisableAntiforgery();
        //ellers ved billedfil brug da
        pathBuilder.MapGet("/{imageId}",
            async ([FromRoute] string imageId, [FromServices] IAzureClientFactory<BlobServiceClient> clientFactory, [FromServices] IImageService _imageService) =>
            {
                //insert password restrictions here 🐿️
                var cts = new CancellationTokenSource(5000);

                //enters images

                var containerClient = _imageService.createContainer(clientFactory);
                BlobClient blobClient = containerClient.GetBlobClient(imageId + ".json");
                if (!blobClient.Exists(cts.Token).ToString()
                        .Contains("404")) //checks if the blobClient is empty/couldn't find the image of that format
                {
                    using var memoryStream = new MemoryStream();
                    await blobClient.DownloadToAsync(memoryStream);
                    return Results.File(memoryStream.ToArray(),
                        "application/json"); //because it is a return statement the for-loop will not continue after finding the image
                }

                

                //will only reach here if it cannot find an image with the id of the correct file type, or else the request will terminate inside the for-loop
                Console.WriteLine("Cannot retrieve image because it doesn't exist");
                return Results.StatusCode(404);

            }).DisableAntiforgery();
        
        pathBuilder.MapDelete("/{imageId}", async (string imageId, [FromServices] IAzureClientFactory<BlobServiceClient> clientFactory, [FromServices] IImageService _imageService) =>
        {
            var cts = new CancellationTokenSource(5000);

            var containerClient = _imageService.createContainer(clientFactory);
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

        }).DisableAntiforgery();

        pathBuilder.MapGet("/filter/{category}",//returns all images of a certain category
            async (string category, [FromServices] IAzureClientFactory<BlobServiceClient> clientFactory, [FromServices] IImageService _imageService) =>
            {
                var cts = new CancellationTokenSource(5000);

                var containerClient = _imageService.createContainer(clientFactory);

                var listOfFiles = containerClient.GetBlobsAsync().AsPages();//all data inside of blobContainer
              
                HashSet<string> collection = new HashSet<string>();
                await foreach (Page<BlobItem> blobPage in listOfFiles)
                {
                    foreach (BlobItem blobItem in blobPage.Values)//every image found
                    {
                        //goes through all images and check for the category
                        var blobClient = containerClient.GetBlobClient(blobItem.Name);
                        string jsonString = await _imageService.convertToJSONString(blobClient);
                        var imageObject = System.Text.Json.JsonSerializer.Deserialize<ImageModel>(jsonString);//deserialize so it becomes imageModel
                        if (imageObject == null)
                        {
                            throw new Exception("image object is null");}
                        if (imageObject.Category == category)
                        {
                            collection.Add(jsonString);
                        }
                    }

                }

                if (collection.Count() == 0)
                {
                    Console.WriteLine("No pictures have this category");
                    //TODO return proper error code
                    //return Results.StatusCode(404);
                }
                return collection.ToArray();//returns array of the JSON files as strings


               



            }).DisableAntiforgery();
        
        pathBuilder.MapGet("/datasets", async (AnnotationsDbContext context, [FromServices] IImageService _imageService) =>//returns all existing datasets
            {
                var cts = new CancellationTokenSource(5000);

                //Datasets from DBContext are transformed to DatasetModels
                var datasets = await context.Datasets
                    .Select(u => new DatasetModel()
                    {
                        Id = u.Id,
                        ImageIds = u.ImageIds,
                        Category = u.Category,
                        AnnotatedBy= u.AnnotatedBy,
                        ReviewedBy = u.ReviewedBy
                    })
                    .ToListAsync();
                
                //DatasetModel list is converted to Array for sending to Blazor front-end
                DatasetModel[] datasetsarray = datasets.ToArray();

                return datasetsarray;

            }).DisableAntiforgery();
        
        pathBuilder.MapGet("/datasets/{dataset}", async (string dataset, AnnotationsDbContext context, [FromServices] IAzureClientFactory<BlobServiceClient> clientFactory, [FromServices] IImageService _imageService) =>
        {//access images of specific dataset
            //TODO: almost identical code as "/filter/{category}" - remove the code duplication
            var cts = new CancellationTokenSource(5000);

            var datasets = context.Datasets
                .Select(u => new DatasetModel()
                {
                    Id = u.Id,
                    ImageIds = u.ImageIds,
                    Category = u.Category,
                    AnnotatedBy= u.AnnotatedBy,
                    ReviewedBy = u.ReviewedBy
                }).Where(DatasetModel => DatasetModel.Id == Int32.Parse(dataset));
                //there is only one dataset with a certain Id, so no point of taking more
            var datasetModel = await datasets.FirstOrDefaultAsync();

            if (datasetModel == null)
            {
                throw new Exception("No dataset found");
            };
            
            HashSet<string> collection = new HashSet<string>();
            var containerClient = _imageService.createContainer(clientFactory);

            //foreach (DatasetModel datasetModel in datasets)//guaranteed to only be one dataset in "datasets", so this is not linear time. 
            //{//there is a better way of doing this
            
                foreach (int ids in datasetModel.ImageIds)
                {
                    Console.WriteLine(ids);
                    BlobClient blobClient = containerClient.GetBlobClient(ids + ".json");
                    if (!blobClient.Exists(cts.Token).ToString()
                            .Contains("404")) //checks if the blobClient is empty/couldn't find the image of that format
                    {
                        string jsonString = await _imageService.convertToJSONString(blobClient);

                        collection.Add(jsonString);
                    }
                    else
                    {
                        Console.WriteLine("Cannot retrieve image because it doesn't exist");
                        break;
                    }
                }
            //}
            return collection.ToArray();//returns array of all images as JSON strings
            

        }).DisableAntiforgery();        
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
}