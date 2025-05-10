using Annotations.API.Services;
using Annotations.Core.Entities;
using Annotations.Core.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;


namespace Annotations.API.Groups;

public static class AnnotationEndpoints
{
    /// <summary>
    /// Where all the annotations endpoints are initialized to their respective handler.
    /// The base endpoint for annotations is /images/annotations. 
    /// </summary>
    /// <param name="pathBuilder"></param>
    public static void MapEndpoints(RouteGroupBuilder pathBuilder)
    {
        // Anti-forgery is disabled. This was decided because the backend will not serve any forms.
        // Anti-forgery measures are covered in the front-end, and by the JWT token protection. 
        pathBuilder.RequireAuthorization().DisableAntiforgery();

        pathBuilder.MapPost("/save", SaveAnnotationHandler);

        pathBuilder.MapGet("/{annotationId}", RetrieveAnnotationHandler);

        //pathBuilder.MapDelete("/{annotationId}", DeleteAnnotation);

        pathBuilder.MapGet("/", GetAnnotationsByImageHandler);

        pathBuilder.MapGet("/exception",
            () =>
            {
                throw new InvalidOperationException(
                    "Exception has been raised in the API. Look for further details in the log.");
            });
    }
    
    
    
    /// <summary>
    /// When completed in the front end, a vessel annotation model, 
    /// containing its vessel points and segments, is retrieved via Post request.
    /// The model is forwarded to the service class to be saved in the database
    /// as an entity.
    /// </summary>
    /// <param name="annotationTree"> The model for one vessel tree. </param>
    /// <param name="_annotationService"> An annotation service instance. </param>
    /// <returns> Status code 200 OK for success, status 500 "Internal Server Error" for failure. </returns>
    public static Task<IResult> SaveAnnotationHandler(VesselAnnotationModel annotationTree,
        [FromServices] IAnnotationService _annotationService)
    {
        var saved = _annotationService.SaveAnnotationToDatabase(annotationTree).Result;

        if (saved)
        {
            return Task.FromResult(Results.StatusCode(200));
        }
        
        return Task.FromResult(Results.StatusCode(500));
        //TODO: Think more about what statuscode to return on error,
        //and how to do error handling if tree has not been saved?
    }
    
    
    
    /// <summary>
    /// Call upon the Annotation Service to retrieve a specific annotation tree.
    /// </summary>
    /// <param name="annotationId"> Id of the annotation tree to retrieve. </param>
    /// <param name="_annotationService"> An instance of the Annotation Service class. </param>
    /// <returns> The retrieved annotation tree. </returns>
    public static async Task<Annotation> RetrieveAnnotationHandler([FromRoute] int annotationId,
        [FromServices] IAnnotationService _annotationService)
    {
        Annotation annotation = await _annotationService.GetAnnotationFromId(annotationId);
        return annotation;
    }
    
    
    
    /// <summary>
    /// The handler for retrieving all annotations from a specified image.
    /// All vessel annotations containing the image path from query,
    /// is retrieved from the database.
    /// </summary>
    /// <param name="imagePath"> Path of the image containing the retrievable annotations. </param>
    /// <param name="context"> The SQLite database context. </param>
    /// <param name="annotationService"> An annotation service instance. </param>
    /// <returns> The list of vessel annotation models retrieved from the specified image. </returns>
    public static async Task<IResult> GetAnnotationsByImageHandler(
        [FromQuery] string imagePath,
        [FromServices] AnnotationsDbContext context,
        [FromServices] IAnnotationService annotationService)
    {
        var annotations = await context.VesselAnnotation
            .Include(a => a.Points)
            .Include(a => a.Segments)
            .ThenInclude(s => s.StartPoint)
            .Include(a => a.Segments)
            .ThenInclude(s => s.EndPoint)
            .Where(a => a.ImagePath == imagePath)
            .ToListAsync();

        var models = VesselModelSupport.ConvertVesselAnnotationsToVesselAnnotationModels(annotations);

        return Results.Ok(models);
    }
}
