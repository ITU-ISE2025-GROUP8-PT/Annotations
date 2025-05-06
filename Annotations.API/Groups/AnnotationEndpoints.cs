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

        pathBuilder.MapGet("/", GetAnnotationsByImage);

        pathBuilder.MapGet("/exception",
            () =>
            {
                throw new InvalidOperationException(
                    "Exception has been raised in the API. Look for further details in the log.");
            });
    }
    
    
    
    public static Task<IResult> SaveAnnotation(VesselAnnotationModel annotationTree, AnnotationsDbContext context,
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
    
    public static async Task<IResult> GetAnnotationsByImage(
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

        var models = annotations.Select(a => new VesselAnnotationModel
        {
            Id = a.Id,
            ImagePath = a.ImagePath,
            Description = a.Description,
            Type = a.Type,
            Points = a.Points.Select(p => new VesselPointModel
            {
                Id = p.Id,
                X = p.X,
                Y = p.Y,
                IsVisible = p.IsVisible
            }).ToList(),
            Segments = a.Segments.Select(s => new VesselSegmentModel
            {
                Id = s.Id,
                StartPoint = new VesselPointModel
                {
                    Id = s.StartPoint.Id,
                    X = s.StartPoint.X,
                    Y = s.StartPoint.Y,
                    IsVisible = s.StartPoint.IsVisible
                },
                EndPoint = new VesselPointModel
                {
                    Id = s.EndPoint.Id,
                    X = s.EndPoint.X,
                    Y = s.EndPoint.Y,
                    IsVisible = s.EndPoint.IsVisible
                },
                Thickness = s.Thickness,
                IsVisible = s.IsVisible
            }).ToList()
        }).ToList();

        return Results.Ok(models);
    }
}