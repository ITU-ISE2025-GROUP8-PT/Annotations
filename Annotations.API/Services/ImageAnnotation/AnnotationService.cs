using Annotations.Core.Entities;
using Annotations.Core.Models;
using Annotations.Core.VesselObjects;
using Microsoft.EntityFrameworkCore;


namespace Annotations.API.Services.ImageAnnotation;


/// <summary>
/// Defines a service for accessing annotations.
/// </summary>
public interface IAnnotationService
{
    public Task<bool> SaveAnnotationToDatabase(VesselAnnotationModel annotationTree);
    public Task<Annotation> GetAnnotationFromId(int annotationId);
}



public class AnnotationService(AnnotationsDbContext context) : IAnnotationService
{
    /// <summary>
    /// Receives a VesselAnnotationModel from the AnnotationEndpoints class.
    /// Converts the model into a VesselAnnotation database entity, saves
    /// it to the database and returns the result of this as a bool.
    /// </summary>
    /// <param name="annotationTree"></param>
    /// <returns>A bool status of the save operation</returns>
    public async Task<bool> SaveAnnotationToDatabase(VesselAnnotationModel annotationTree)
    {
        List<VesselPoint> pointsList = VesselModelSupport.ConvertVesselPointModelToVesselPoint(annotationTree.Points);
        List<VesselSegment> segmentList = VesselModelSupport.ConvertVesselSegmentModelToVesselPoint(annotationTree.Segments);
        
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
        
        var saved = await context.SaveChangesAsync();
        
        
        // If anything has been saved to the database, the value of the save operation
        // will be more than 0. Therefore, true will be a successful operation.
        return saved > 1;
    }
    

    
    /// <summary>
    /// Gets and returns an annotation from the database, which matches the
    /// provided parameter annotationId.
    /// </summary>
    /// <param name="annotationId"></param>
    /// <returns>The annotation associated with the Id</returns>
    public async Task<Annotation> GetAnnotationFromId(int annotationId)
    {
        IQueryable<Annotation> response = context.Annotation
            .Select(annotation => annotation)
            .Where(annotation => annotation.Id == annotationId);
        
        return await response.FirstAsync();
    }
}
