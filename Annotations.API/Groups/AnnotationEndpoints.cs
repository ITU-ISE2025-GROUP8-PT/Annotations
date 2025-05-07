using Annotations.API.Services;
using Annotations.Core.Entities;
using Annotations.Core.Models;
using Annotations.Core.VesselObjects;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Annotations.API.Groups;

public static class AnnotationEndpoints
{

    /// <summary>
    /// Where all the endpoints are initialized to their respective handler. 
    /// </summary>
    /// <param name="pathBuilder"></param>
    public static void MapEndpoints(RouteGroupBuilder pathBuilder)
    {
        pathBuilder.RequireAuthorization().DisableAntiforgery();

        pathBuilder.MapPost("/save", SaveAnnotationHandler); //should url not be this?

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
    /// When completed, a vessel annotation is saved to the database,
    /// containing its vessel points and segments.
    /// All points and segments are thereby stored in its respective tree.
    /// </summary>
    /// <param name="annotationTree"> The model for one vessel tree. </param>
    /// <param name="context"> The SQLite database context. </param>
    /// <param name="_annotationService"> An annotation service instance. </param>
    /// <returns> A result with status code 200 OK. </returns>
    public static Task<IResult> SaveAnnotationHandler(VesselAnnotationModel annotationTree, AnnotationsDbContext context,
        [FromServices] IAnnotationService _annotationService)
    {
        /*if doesnotwork
        {
            return Results.StatusCode(422);
            //TODO: how should the end user see this?
        }*/

        List<VesselPoint> pointsList = _annotationService.ConvertVesselPointModelToVesselPoint(annotationTree.Points);
        List<VesselSegment> segmentList = _annotationService.ConvertVesselSegmentModelToVesselPoint(annotationTree.Segments);
        
        
        context.Add(new VesselAnnotation
        {
            Id = annotationTree.Id,
            ImagePath = annotationTree.ImagePath,
            Points = pointsList,
            Segments = segmentList,
            Description = annotationTree.Description,
            Type = annotationTree.Type,
            IsVisible = annotationTree.IsVisible
        });
        
        context.SaveChanges();
        
        
        return Task.FromResult(Results.StatusCode(200));
    }
    
    
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

        var models = annotationService.GetAnnotationsByImage(annotations);

        return Results.Ok(models);
    }
}