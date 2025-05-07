using Annotations.Core.Models;
using Annotations.Core.VesselObjects;

namespace Annotations.API.Services;



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

}