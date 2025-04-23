using System.ComponentModel.DataAnnotations;

namespace Annotations.Core.Entities;

public class VesselPoint
{
    [Key]
    public required int PointId { get; set; }
    [Required]
    public required int X { get; set; }
    [Required]
    public required int Y { get; set; }
    public string Text { get; set; }
    public string Type { get; set; }
}

public class VesselSegment
{
    [Key]
    public required int SegmentId { get; set; }
    [Required]
    public required VesselPoint? StartPoint { get; set; }
    [Required]
    public required VesselPoint? EndPoint { get; set; }
    public string? Type { get; set; }
    public double Thickness { get; set; }
    public string Text { get; set; }
}

public class VesselTree
{
    [Key]
    public int VesselTreeId { get; set; }
    [Required]
    public required int ImageId { get; set; }
    [Required]
    public required User CreatedBy { get; set; }
    [Required]
    public required ICollection<VesselSegment> Segments { get; set; } = [];
}