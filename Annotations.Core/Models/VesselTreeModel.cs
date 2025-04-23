using System.ComponentModel.DataAnnotations;
using Annotations.Core.Entities;

namespace Annotations.Core.Models;

public class VesselTreeModel
{
    public int VesselTreeId { get; set; }
    public required int ImageId { get; set; }
    public required User CreatedBy { get; set; }
    public required ICollection<VesselSegmentModel> Segments { get; set; } = [];
}

public class VesselPointModel
{
    public required int PointId { get; set; }
    public required int X { get; set; }
    public required int Y { get; set; }
    public string Text { get; set; }
    public string Type { get; set; }
}

public class VesselSegmentModel
{
    public required int SegmentId { get; set; }
    public required VesselPoint? StartPoint { get; set; }
    public required VesselPoint? EndPoint { get; set; }
    public string? Type { get; set; }
    public double Thickness { get; set; }
    public string Text { get; set; }
}

