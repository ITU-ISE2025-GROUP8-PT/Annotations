using Annotations.Core.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Annotations.API.Groups;

public static class ImagesGroup
{
    public static void MapEndpoints(RouteGroupBuilder pathBuilder)
    {
        pathBuilder.MapGet("/", () => "Upload an image here!");
        //Delete an image (kun image ID), Get an image by imageId (kun image ID), Update / PATCH an image (title og text) 
        
        // hvis JSON objektet skal returneres brug da:
        /*
        pathBuilder.MapGet("/{imageId}", async ([FromRoute] int imageId, AnnotationsDbContext context) =>
        {
            var image = await context.Images.FindAsync(imageId);
            return image is not null ? Results.Ok(image) : Results.NotFound();
        });
        */

        //ellers ved billedfil brug da
        pathBuilder.MapGet("/{imageId}", async ([FromRoute] int imageId, AnnotationsDbContext context) =>
        {
            //indsæt adgangskontol her 🐿️
            var image = await context.Images.FindAsync(imageId);
            if (image is null)
            {
                return Results.NotFound();
            }

            return Results.File(image.ImageData, "image/png");
        });

        pathBuilder.MapGet("/exception",
            () =>
            {
                throw new InvalidOperationException(
                    "Exception has been raised in the API. Look for further details in the log.");
            });
    }
}