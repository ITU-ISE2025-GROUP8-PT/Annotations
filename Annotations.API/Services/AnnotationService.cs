using Annotations.Core.Entities;
using Annotations.Core.Models;
using Annotations.Core.VesselObjects;

namespace Annotations.API.Services;



/// <summary>
/// Defines a service for accessing annotations.
/// </summary>
public interface IAnnotationService
{
    public bool SaveAnnotationToDatabase(VesselAnnotationModel annotationTree);
    public List<VesselPoint> ConvertVesselPointModelToVesselPoint(List<VesselPointModel> points);
    public List<VesselSegment> ConvertVesselSegmentModelToVesselPoint(List<VesselSegmentModel> segments);
    public Task<Annotation> GetAnnotationFromId(int annotationId);
    public List<VesselAnnotationModel> GetAnnotationsByImage(List<VesselAnnotation> annotations);
}



public class AnnotationService(AnnotationsDbContext context) : IAnnotationService
{
    
    /// <summary>
    /// Receives a VesselAnnotationModel from the AnnotationEndpoints class.
    /// Converts the model into a VesselAnnotation database entity, saves
    /// it to the database and returns the result of this as a bool.
    /// </summary>
    /// <param name="annotationTree"></param>
    /// <returns></returns>
    public bool SaveAnnotationToDatabase(VesselAnnotationModel annotationTree)
    {
        List<VesselPoint> pointsList = ConvertVesselPointModelToVesselPoint(annotationTree.Points);
        List<VesselSegment> segmentList = ConvertVesselSegmentModelToVesselPoint(annotationTree.Segments);
        
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
        
        var saved = context.SaveChangesAsync();
        
        
        // If anything has been saved to the database, the value of the save operation
        // will be more than 0. Therefore, true will be a successful operation.
        return saved.Result > 1;
    }
    
    
    
    /// <summary>
    /// Conversion from VesselPointModel to VesselPoint entity, for saving in the database.
    /// </summary>
    /// <param name="points"></param>
    /// <returns></returns>
    public List<VesselPoint> ConvertVesselPointModelToVesselPoint(List<VesselPointModel> points)
    {
        List<VesselPoint> pointsList = new  List<VesselPoint>();
        
        foreach (VesselPointModel pointModel in points)
        {
            pointsList.Add(new VesselPoint
            {
                Id = pointModel.Id,
                X = pointModel.X,
                Y = pointModel.Y,
                IsVisible = pointModel.IsVisible
            });
        }

        return pointsList;
    }

    
    
    /// <summary>
    /// Conversion from VesselSegmentModel to VesselSegment entity, for saving in the database.
    /// </summary>
    /// <param name="segments"></param>
    /// <returns></returns>
    public List<VesselSegment> ConvertVesselSegmentModelToVesselPoint(List<VesselSegmentModel> segments)
    {
        List<VesselSegment> segmentList = new  List<VesselSegment>();
        
        foreach (VesselSegmentModel segmentModel in segments)
        {
            segmentList.Add(new VesselSegment
            {
                Id = segmentModel.Id,
                StartPoint = new VesselPoint
                {
                    Id = segmentModel.StartPoint.Id,
                    X = segmentModel.StartPoint.X,
                    Y = segmentModel.StartPoint.Y,
                    IsVisible = segmentModel.EndPoint.IsVisible
                },
                EndPoint = new VesselPoint
                {
                    Id = segmentModel.EndPoint.Id,
                    X = segmentModel.EndPoint.X,
                    Y = segmentModel.EndPoint.Y,
                    IsVisible = segmentModel.EndPoint.IsVisible
                },
                Thickness = segmentModel.Thickness,
                IsVisible = segmentModel.IsVisible
            });
        }

        return segmentList;
    }


    
    /// <summary>
    /// Gets and returns an annotation from the database, which matches the
    /// provided parameter annotationId.
    /// </summary>
    /// <param name="annotationId"></param>
    /// <returns></returns>
    public Task<Annotation> GetAnnotationFromId(int annotationId)
    {
        IQueryable<Annotation> response = context.Annotation
            .Select(annotation => annotation)
            .Where(annotation => annotation.Id == annotationId);
        
        return Task.FromResult(response.First());
    }

    
    
    /// <summary>
    /// Initialisation of the VesselAnnotationModels retrieved from a specified image.
    /// </summary>
    /// <param name="annotations"> A list of VesselAnnotation entities. </param>
    /// <returns> The list of VesselAnnotationModels, containing their points and segments. </returns>
    public List<VesselAnnotationModel> GetAnnotationsByImage(List<VesselAnnotation> annotations)
    {
        var models = annotations.Select(a => new VesselAnnotationModel
        {
            Id = a.Id,
            ImagePath = a.ImagePath,
            Description = a.Description,
            Type = a.Type,
            IsVisible = a.IsVisible,
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

        return models;
    }
}