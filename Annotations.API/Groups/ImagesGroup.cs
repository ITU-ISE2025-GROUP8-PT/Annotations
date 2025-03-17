using Annotations.Core.Models;
using Microsoft.AspNetCore.Http.Extensions;
using Annotations.Core.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Microsoft.AspNetCore.Http.HttpResults;

namespace Annotations.API.Groups;

public static class ImagesGroup
{
    private static int counter = 0;//change this - it gets reset every time the program resets
    private record ValidationResponse(bool Success, string Message);
    //TODO: DONT DO THIS
    private static string connectionString =
        "AccountName=devstoreaccount1;AccountKey=Eby8vdM02xNOcqFlqUwJPLlmEtlCDXJ1OUzFT50uSRZ6IFsuFq2UVErCz4I6tq/K1SZFPTOtr/KBHBeksoGMGw==;BlobEndpoint=http://127.0.0.1:10000/devstoreaccount1;";
    private static BlobServiceClient blobServiceClient = new BlobServiceClient(connectionString);
    private static BlobContainerClient containerClient = blobServiceClient.GetBlobContainerClient("images");
    private static string[] ArrayOfFileExtension = {"jpg", "jpeg", "png"};

    /// <summary>
    /// Helping method that validates an image based on type, size, and also checks if it even contains anything
    /// Images can be JPEG, PNG and JPG, and everything else gets rejected
    /// </summary>
    /// <param name="file"></param>
    /// <returns>ValidationResponse - a record that contains a boolean of whether the image is validated, and a message that describes either what went wrong, or that it was successful</returns>
    private static ValidationResponse ValidateImage(IFormFile file)//temporary location
    {
        if (file.ContentType != "image/jpeg" && file.ContentType != "image/png" &&  file.ContentType != "image/jpg")
        {
            return new ValidationResponse(false, "File is not a valid image.");
        } 
        if (file.Length > 50 * 1024 * 1024) {//50MB
            return new ValidationResponse(false, "File is too large.");
        }
        if (file.Length == 0) 
        { 
            return new ValidationResponse(false, "File doesn't exist.");
        }
        //upload image to db
        return new ValidationResponse(true ,"Uploaded image successfully.");
        
    }
    public static void MapEndpoints(RouteGroupBuilder pathBuilder)
    {
        pathBuilder.MapGet("/", () => "Upload an image here!");

       
        
        
        //This is the upload endpoint where the image first gets validated, and then gets uploaded into your local Azurite BlobStorage
        //The image gets saved in the database as the same file type it was uploaded as
        pathBuilder.MapPost("/upload", async (IFormFile image) =>
        {
          
            ValidationResponse response = ValidateImage(image);
            if (!response.Success)
            {
                Console.WriteLine("rejecting image");
                //TODO: how should the end user see this?
            }
            else
            {
                //fileExtension will always be a proper fileExtension because of the ValidateImage method
                string fileExtension = "empty";//TODO: dont do this
                switch (image.ContentType)
                {
                    case "image/jpeg":
                        fileExtension = ".jpeg";
                        break;
                    case "image/png":
                        fileExtension = ".png";
                        break;
                    case "image/jpg":
                        fileExtension = ".jpg";
                        break;
                }
                
                BlobClient imageBlobClient = containerClient.GetBlobClient($"{counter}{fileExtension}");
                counter++;
                using (var fileStream = image.OpenReadStream())
                {
                    await imageBlobClient.UploadAsync(fileStream, overwrite: true);
                }
            }


        }).DisableAntiforgery();
        //ellers ved billedfil brug da
        pathBuilder.MapGet("/{imageId}", async ([FromRoute] string imageId, AnnotationsDbContext context) =>
        {
            //indsæt adgangskontol her 🐿️
            var cts = new CancellationTokenSource(5000);

            foreach (string fileExtension in ArrayOfFileExtension)
            {
                BlobClient blobClient = containerClient.GetBlobClient(imageId + "." + fileExtension);
                if (!blobClient.Exists(cts.Token).ToString().Contains("404"))
                {
                    using var memoryStream = new MemoryStream();
                    await blobClient.DownloadToAsync(memoryStream);
                    return Results.File(memoryStream.ToArray(), "image/" + fileExtension);
                }

            }
            Console.WriteLine("Cannot retrieve image because it doesn't exist");
            return Results.StatusCode(404);
            
        });
        pathBuilder.MapPost("/delete", async (string imageId) =>
        {
            var cts = new CancellationTokenSource(5000);

            foreach (string fileExtension in ArrayOfFileExtension)
            {
                BlobClient blobClient = containerClient.GetBlobClient(imageId + "." + fileExtension);
                if (!blobClient.Exists(cts.Token).ToString().Contains("404"))
                {
                    await blobClient.DeleteAsync(snapshotsOption: DeleteSnapshotsOption.IncludeSnapshots);
                    Console.WriteLine("image deleted successfully");
                    return Results.StatusCode(204);
                }

            }
            Console.WriteLine("Cannot delete image because it doesn't exist");
            return Results.StatusCode(404);

        }).DisableAntiforgery();

        pathBuilder.MapGet("/exception",
            () =>
            {
                throw new InvalidOperationException(
                    "Exception has been raised in the API. Look for further details in the log.");
            });
    }
}