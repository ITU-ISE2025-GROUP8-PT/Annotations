namespace Annotations.Core.Models;

public class VesselAnnotationModel
{
    public required int Id { get; set; }
    
    public required List<VesselPointModel> Points { get; set; }
    
    public required List<VesselSegmentModel> Segments { get; set; }
}