using Annotations.Core.Entities;
using Annotations.Core.Models;
using Annotations.Core.VesselObjects;

namespace Annotations.API.Services;

public interface IAnnotationService
{
    public List<VesselPoint> ConvertVesselPointModelToVesselPoint(List<VesselPointModel> points);
    public List<VesselSegment> ConvertVesselSegmentModelToVesselPoint(List<VesselSegmentModel> segments);
    public Task<Annotation> GetAnnotationFromId(int annotationId);
    
}

public class AnnotationService : IAnnotationService
{
    private readonly AnnotationsDbContext _context;
    public List<VesselPoint> ConvertVesselPointModelToVesselPoint(List<VesselPointModel> points)
    {
        List<VesselPoint> PointsList = new  List<VesselPoint>();
        
        foreach (VesselPointModel PointModel in points)
        {
            PointsList.Add(new VesselPoint()
            {
                Id = PointModel.Id,
                X = PointModel.X,
                Y = PointModel.Y,
                IsVisible = PointModel.IsVisible
            });
        }

        return PointsList;
    }


    public List<VesselSegment> ConvertVesselSegmentModelToVesselPoint(List<VesselSegmentModel> segments)
    {
        List<VesselSegment> SegmentList = new  List<VesselSegment>();
        
        foreach (VesselSegmentModel SegmentModel in segments)
        {
            SegmentList.Add(new VesselSegment()
            {
                Id = SegmentModel.Id,
                StartPoint = new VesselPoint()
                {
                    Id = SegmentModel.StartPoint.Id,
                    X = SegmentModel.StartPoint.X,
                    Y = SegmentModel.StartPoint.Y,
                    IsVisible = SegmentModel.EndPoint.IsVisible
                },
                EndPoint = new VesselPoint()
                {
                    Id = SegmentModel.EndPoint.Id,
                    X = SegmentModel.EndPoint.X,
                    Y = SegmentModel.EndPoint.Y,
                    IsVisible = SegmentModel.EndPoint.IsVisible
                },
                Thickness = SegmentModel.Thickness,
                IsVisible = SegmentModel.IsVisible
            });
        }

        return SegmentList;
    }


    public Task<Annotation> GetAnnotationFromId(int annotationId)
    {
        IQueryable<Annotation> response = _context.Annotation
            .Select(annotation => annotation)
            .Where(annotation => annotation.Id == annotationId);
        
        return Task.FromResult(response.First());
    }
}