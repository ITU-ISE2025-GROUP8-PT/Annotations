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
    private record ValidationResponse(bool Success, string ErrorMessage);
    private static ValidationResponse ValidateImage(IFormFile file)//temporary location
    {
        Console.WriteLine("validating image");
        if (file.ContentType != "image/jpeg" && file.ContentType != "image/png" &&  file.ContentType != "image/jpg")
        {
            Console.WriteLine("Invalid image format");
            return new ValidationResponse(false, "File is not a valid image.");
        } 
        if (file.Length > 5 * 1024 * 1024) {
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

       
        
        //Husk: Delete an image (kun image ID), Get an image by imageId (kun image ID), Update / PATCH an image (title og text) fra openapi dokumentet
        
        // hvis JSON objektet skal returneres brug da:
        /*
        pathBuilder.MapGet("/{imageId}", async ([FromRoute] int imageId, AnnotationsDbContext context) =>
        {
            var image = await context.Images.FindAsync(imageId);
            return image is not null ? Results.Ok(image) : Results.NotFound();
        });
        */
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
                Console.WriteLine("Image uploading.");
                var connectionString =
                    "AccountName=devstoreaccount1;AccountKey=Eby8vdM02xNOcqFlqUwJPLlmEtlCDXJ1OUzFT50uSRZ6IFsuFq2UVErCz4I6tq/K1SZFPTOtr/KBHBeksoGMGw==;DefaultEndpointsProtocol=http;BlobEndpoint=http://127.0.0.1:10000/devstoreaccount1;QueueEndpoint=http://127.0.0.1:10001/devstoreaccount1;TableEndpoint=http://127.0.0.1:10002/devstoreaccount1;";
                BlobServiceClient blobServiceClient = new BlobServiceClient(connectionString);
                BlobContainerClient containerClient = blobServiceClient.GetBlobContainerClient("images");
            
            
                BlobClient imageBlobClient = containerClient.GetBlobClient($"{counter}.jpg");
                counter++;
                using (var fileStream = image.OpenReadStream())
                {
                    await imageBlobClient.UploadAsync(fileStream, overwrite: true);
                    Console.WriteLine("Uploaded image successfully.");
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