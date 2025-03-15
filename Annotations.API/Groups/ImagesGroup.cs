using Annotations.Core.Models;
using Microsoft.AspNetCore.Http.Extensions;
using Annotations.Core.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Azure.Storage.Blobs;
using Microsoft.AspNetCore.Http.HttpResults;

namespace Annotations.API.Groups;

public static class ImagesGroup
{
    private static int counter = 0;//change this - it gets reset every time the program resets
    private record ValidationResponse(bool Success, string Message);
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
                //this connectionString was based on Nickie's own local Azurite - replace this with your own connection string
                //TODO: make connectionString not local
                var connectionString =
                    "AccountName=devstoreaccount1;AccountKey=Eby8vdM02xNOcqFlqUwJPLlmEtlCDXJ1OUzFT50uSRZ6IFsuFq2UVErCz4I6tq/K1SZFPTOtr/KBHBeksoGMGw==;DefaultEndpointsProtocol=http;BlobEndpoint=http://127.0.0.1:10000/devstoreaccount1;QueueEndpoint=http://127.0.0.1:10001/devstoreaccount1;TableEndpoint=http://127.0.0.1:10002/devstoreaccount1;";
                BlobServiceClient blobServiceClient = new BlobServiceClient(connectionString);
                BlobContainerClient containerClient = blobServiceClient.GetBlobContainerClient("images");

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
        pathBuilder.MapGet("/{imageId}", async ([FromRoute] int imageId, AnnotationsDbContext context) =>
        {
            //indsæt adgangskontol her 🐿️
            var cts = new CancellationTokenSource(5000);

            try
            {
                var image = await context.Images.FindAsync(imageId, cts.Token);
                if (image is null)
                {
                    return Results.NotFound();
                }
                return Results.File(image.ImageData, "image/png");
            }
            catch (OperationCanceledException)
            { return Results.StatusCode(408); }
        });

        pathBuilder.MapGet("/exception",
            () =>
            {
                throw new InvalidOperationException(
                    "Exception has been raised in the API. Look for further details in the log.");
            });
    }
}