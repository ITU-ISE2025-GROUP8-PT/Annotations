using Annotations.Core.Models;
using Microsoft.AspNetCore.Http.Extensions;
using Annotations.Core.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Annotations.API.Groups;

public static class ImagesGroup
{
    public static void MapEndpoints(RouteGroupBuilder pathBuilder)
    {
        pathBuilder.MapGet("/", () => "Upload an image here!");

        pathBuilder.MapPost("/imagess", async (HttpRequest request) =>
        {
            IFormFile file = request.Form.Files[0];
            if (file.ContentType != "image/jpeg" || file.ContentType != "image/png")
            {
                return Results.BadRequest("File is not a valid image.");
            } else if (file.Length > 5 * 1024 * 1024)
            {
                return Results.BadRequest("File is too large.");
            }
            else if (file.Length == 0) 
            { 
                Results.BadRequest("File doesn't exist.");
            }
            else
            {
                //upload image to db
                return Results.Ok("Uploaded image successfully.");
            }
            return Results.BadRequest(string.Empty);

            //var url = request.GetDisplayUrl();

            //var path = request.Path;
            

        });
        
        //Husk: Delete an image (kun image ID), Get an image by imageId (kun image ID), Update / PATCH an image (title og text) fra openapi dokumentet
        
        // hvis JSON objektet skal returneres brug da:
        /*
        pathBuilder.MapGet("/{imageId}", async ([FromRoute] int imageId, AnnotationsDbContext context) =>
        {
            var image = await context.Images.FindAsync(imageId);
            return image is not null ? Results.Ok(image) : Results.NotFound();
        });
        */
        pathBuilder.MapPost("/upload", async (string url, AnnotationsDbContext context) =>
        {
            using HttpClient client = new HttpClient(); 
            Uri uri = new Uri(url);
            byte[] response = client.GetByteArrayAsync(uri).Result;
            
            var image = new Image
            {
                Id = 03,
                Title = "idk3",
                Description = "idk4",
                ImageData = response
            };
            await context.Images.AddAsync(image);
            await context.SaveChangesAsync();
            return Results.Ok(new { message = "Image uploaded successfully" });
        });
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