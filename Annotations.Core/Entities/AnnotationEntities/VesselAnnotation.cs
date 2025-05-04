using System.ComponentModel.DataAnnotations;
namespace Annotations.Core.VesselObjects;

public class VesselAnnotation 
{
    [Required]
    public required int Id { get; set; }
    
    [Required]
    public required List<VesselPoint> Points { get; set; }
    
    [Required]
    public required List<VesselSegment> Segments { get; set; }
    
    [Required]
    public required string Description { get; set; }
}