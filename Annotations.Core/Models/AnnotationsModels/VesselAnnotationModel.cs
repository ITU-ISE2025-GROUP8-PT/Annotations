namespace Annotations.Core.Models;

public class VesselAnnotationModel
{
    public int Id { get; set; }
    public required string ImagePath { get; set; }
    public required List<VesselPointModel> Points { get; set; }
    public required List<VesselSegmentModel> Segments { get; set; }
    public required string Description { get; set; }
    public required string Type { get; set; }
    public required bool IsVisible { get; set; } = true;
}