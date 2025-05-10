using Annotations.Core.Models;
using Annotations.Core.VesselObjects;


namespace Annotations.API.Services.ImageAnnotation;


/// <summary>
/// Static helper methods for the AnnotationService class.
/// </summary>
public static class VesselModelSupport
{
    /// <summary>
    /// Conversion from VesselPointModel to VesselPoint entity, for saving in the database.
    /// </summary>
    /// <param name="points"></param>
    /// <returns>The converted list of VesselPoints</returns>
    public static List<VesselPoint> ConvertVesselPointModelToVesselPoint(List<VesselPointModel> points)
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
    /// <returns>The converted list of VesselSegments</returns>
    public static List<VesselSegment> ConvertVesselSegmentModelToVesselPoint(List<VesselSegmentModel> segments)
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
    /// Initialisation of the VesselAnnotationModels retrieved from a specified image.
    /// </summary>
    /// <param name="annotations"> A list of VesselAnnotation entities. </param>
    /// <returns> The list of VesselAnnotationModels, containing their points and segments. </returns>
    public static List<VesselAnnotationModel> ConvertVesselAnnotationsToVesselAnnotationModels(List<VesselAnnotation> annotations)
    {
        return annotations.Select(a => new VesselAnnotationModel
        {
            Id = a.Id,
            ImagePath = a.ImagePath,
            Description = a.Description,
            Type = a.Type,
            IsVisible = a.IsVisible,
            Points = a.Points.Select(mapToVesselPointModel).ToList(),
            Segments = a.Segments.Select(s => new VesselSegmentModel
            {
                Id = s.Id,
                StartPoint = mapToVesselPointModel(s.StartPoint),
                EndPoint = mapToVesselPointModel(s.EndPoint),
                Thickness = s.Thickness,
                IsVisible = s.IsVisible
            }).ToList()
        }).ToList();
    }



    /// <summary>
    /// Helper function for converting VesselPoint to VesselPointModel.
    /// </summary>
    private static readonly Func<VesselPoint, VesselPointModel> mapToVesselPointModel =
        vp => new VesselPointModel
        {
            Id = vp.Id,
            X = vp.X,
            Y = vp.Y,
            IsVisible = vp.IsVisible
        };
}
