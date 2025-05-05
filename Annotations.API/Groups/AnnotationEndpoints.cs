using Annotations.API.Services;
using Annotations.Core.Entities;
using Annotations.Core.Models;
using Annotations.Core.VesselObjects;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Annotations.API.Groups;

public static class AnnotationEndpoints
{

    public static void MapEndpoints(RouteGroupBuilder pathBuilder)
    {
        pathBuilder.RequireAuthorization().DisableAntiforgery();

        pathBuilder.MapPost("/save", SaveAnnotation); //should url not be this?

        pathBuilder.MapGet("/{annotationId}", RetrieveAnnotation);

        //pathBuilder.MapDelete("/{annotationId}", DeleteAnnotation);

        //pathBuilder.MapGet("/", Annotate);

        pathBuilder.MapGet("/exception",
            () =>
            {
                throw new InvalidOperationException(
                    "Exception has been raised in the API. Look for further details in the log.");
            });
    }
    
    
    
    public static Task<IResult> SaveAnnotation(VesselAnnotationModel AnnotationTree, AnnotationsDbContext context,
        [FromServices] IAnnotationService _annotationService)
    {
        /*if doesnotwork
        {
            return Results.StatusCode(422);
            //TODO: how should the end user see this?
        }*/

        List<VesselPoint> PointsList = _annotationService.ConvertVesselPointModelToVesselPoint(AnnotationTree.Points);
        List<VesselSegment> SegmentList = _annotationService.ConvertVesselSegmentModelToVesselPoint(AnnotationTree.Segments);
        
        
        context.Add(new VesselAnnotation
        {
            Id = AnnotationTree.Id,
            Points = PointsList,
            Segments = SegmentList,
            Description = AnnotationTree.Description,
            Type = AnnotationTree.Type,
        });
        
        context.SaveChanges();
        
        
        return Task.FromResult(Results.StatusCode(200));
    }
    
    
    public static async Task<Annotation> RetrieveAnnotation([FromRoute] int annotationId,
        [FromServices] IAnnotationService _annotationService)
    {
        Annotation annotation = await _annotationService.GetAnnotationFromId(annotationId);
        return annotation;
    }
}