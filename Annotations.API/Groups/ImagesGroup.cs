using Annotations.Core.Models;
using Microsoft.AspNetCore.Http.Extensions;
using Annotations.Core.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.Extensions.Azure;

namespace Annotations.API.Groups;

public static class ImagesGroup
{
    private static int counter = 0;//change this - it gets reset every time the program resets
    private record ValidationResponse(bool Success, string Message);
    
    private static string[] ArrayOfFileExtension = {"png", "jpg", "jpeg"};

    /// <summary>
    /// Helping method that validates an image based on type, size, and also checks if it even contains anything
    /// Images can be JPEG, PNG and JPG, and everything else gets rejected
    /// </summary>
    /// <param name="file"></param>
    /// <returns>ValidationResponse - a record that contains a boolean of whether the image is validated, and a message that describes either what went wrong, or that it was successful</returns>
    private static ValidationResponse ValidateImage(IFormFile file)//temporary location
    {
        
        if (file.Length > 50 * 1024 * 1024) {//50MB
            return new ValidationResponse(false, "File is too large.");
        }
        if (file.Length == 0) 
        { 
            return new ValidationResponse(false, "File doesn't exist.");
        }
        foreach (string FileExtension in ArrayOfFileExtension)
        {
            if (file.ContentType.Contains(FileExtension))
            {
                //if the image is the correct type, it will be uploaded, since it fulfills the other criterias
                //upload image to db
                return new ValidationResponse(true ,"Uploaded image successfully.");
            }
        }
        //if the code reaches this point, then the file type is none of the permitted file types, so an error is thrown
        return new ValidationResponse(false, "File is not a valid image.");
        
    }
    public static void MapEndpoints(RouteGroupBuilder pathBuilder)
    {
        pathBuilder.RequireAuthorization();
        
        //This is the upload endpoint where the image first gets validated, and then gets uploaded into your local Azurite BlobStorage
        //The image gets saved in the database as the same file type it was uploaded as
        pathBuilder.MapPost("/upload", async (IFormFile image, [FromServices] IAzureClientFactory<BlobServiceClient> clientFactory) =>
        {
          
            ValidationResponse response = ValidateImage(image);
            if (!response.Success)
            {
                Console.WriteLine("rejecting image");
                Console.WriteLine(response.Message);
                return Results.StatusCode(422);
                //TODO: how should the end user see this?
            }
            else
            {
                var blobServiceClient = clientFactory.CreateClient("Default");
                BlobContainerClient containerClient = blobServiceClient.GetBlobContainerClient("images");

                //fileExtension will always be a proper fileExtension because of the ValidateImage method
                string fileExtension = "empty";//TODO: dont do this

                foreach (string Filename in ArrayOfFileExtension)
                {
                    if (image.ContentType == "image/" + Filename)
                    {
                        fileExtension = "." + Filename;
                        break;
                    }
                }
                
                BlobClient imageBlobClient = containerClient.GetBlobClient($"{counter}{fileExtension}");
                counter++;
                using (var fileStream = image.OpenReadStream())
                {
                    await imageBlobClient.UploadAsync(fileStream, overwrite: true);
                }

                return Results.StatusCode(200);
            }


        });
        //ellers ved billedfil brug da
        pathBuilder.MapGet("/{imageId}", async ([FromRoute] string imageId, [FromServices] IAzureClientFactory<BlobServiceClient> clientFactory) =>
        {
            //insert password restrictions here 🐿️
            var cts = new CancellationTokenSource(5000);

            var blobServiceClient = clientFactory.CreateClient("Default");
            BlobContainerClient containerClient = blobServiceClient.GetBlobContainerClient("images");

            foreach (string fileExtension in ArrayOfFileExtension)//takes all of the types of files we allow and see if an image of that format exists with the id
            {
                BlobClient blobClient = containerClient.GetBlobClient(imageId + "." + fileExtension);
                if (!blobClient.Exists(cts.Token).ToString().Contains("404"))//checks if the blobClient is empty/couldn't find the image of that format
                {
                    using var memoryStream = new MemoryStream();
                    await blobClient.DownloadToAsync(memoryStream);
                    return Results.File(memoryStream.ToArray(), "image/" + fileExtension);//because it is a return statement the for-loop will not continue after finding the image
                }

            }
            //will only reach here if it cannot find an image with the id of the correct file type, or else the request will terminate inside the for-loop
            Console.WriteLine("Cannot retrieve image because it doesn't exist");
            return Results.StatusCode(404);
            
        });
        
        pathBuilder.MapDelete("/{imageId}", async (string imageId, [FromServices] IAzureClientFactory<BlobServiceClient> clientFactory) =>
        {
            var cts = new CancellationTokenSource(5000);

            var blobServiceClient = clientFactory.CreateClient("Default");
            BlobContainerClient containerClient = blobServiceClient.GetBlobContainerClient("images");

            foreach (string fileExtension in ArrayOfFileExtension)
            {
                BlobClient blobClient = containerClient.GetBlobClient(imageId + "." + fileExtension);
                if (!blobClient.Exists(cts.Token).ToString().Contains("404"))
                {
                    /*A snapshot is a read-only version of a blob that's taken at a point in time. 
                    As of right now, we do not make snapshots of blobs, but it is still possible to manually create.*/
                    await blobClient.DeleteAsync(snapshotsOption: DeleteSnapshotsOption.IncludeSnapshots);
                    Console.WriteLine("image deleted successfully");
                    return Results.StatusCode(204);
                }

            }
            Console.WriteLine("Cannot delete image because it doesn't exist");
            return Results.StatusCode(404);

        });

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