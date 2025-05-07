using System.ComponentModel.DataAnnotations;
namespace Annotations.Core.VesselObjects;

public class VesselAnnotation 
{
    public int Id { get; set; }
    
    [Required]
    public required string ImagePath { get; set; }
    
    [Required]
    public required List<VesselPoint> Points { get; set; }
    
    [Required]
    public required List<VesselSegment> Segments { get; set; }
    
    [Required]
    public required string Description { get; set; }
    
    [Required]
    public required string Type { get; set; }
    
    [Required]
    public required bool IsVisible { get; set; }
}