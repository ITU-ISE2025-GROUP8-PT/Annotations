using Annotations.Core.Entities;
using Annotations.Core.Models;
using Annotations.Core.VesselObjects;

namespace Annotations.API.Services;

public interface IAnnotationService
{
    public List<VesselPoint> ConvertVesselPointModelToVesselPoint(List<VesselPointModel> points);
    public List<VesselSegment> ConvertVesselSegmentModelToVesselPoint(List<VesselSegmentModel> segments);
    public Task<Annotation> GetAnnotationFromId(int annotationId);
    public List<VesselAnnotationModel> GetAnnotationsByImage(List<VesselAnnotation> annotations);
}

public class AnnotationService(AnnotationsDbContext context) : IAnnotationService
{
    /// <summary>
    /// Conversion from VesselPointModel to VesselPoint entity.
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
    /// Conversion from VesselSegmentModel to VesselSegment entity.
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