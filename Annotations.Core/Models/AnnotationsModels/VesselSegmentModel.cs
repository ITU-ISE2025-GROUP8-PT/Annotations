namespace Annotations.Core.Models;

public class VesselSegmentModel
{
    public required int Id { get; set; }
    public required VesselPointModel StartPoint { get; set; }
    public required VesselPointModel EndPoint { get; set; }
    public required double Thickness { get; set; }
    public required bool IsVisible { get; set; }
}