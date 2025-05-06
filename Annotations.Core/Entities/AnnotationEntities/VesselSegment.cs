using System.ComponentModel.DataAnnotations;
namespace Annotations.Core.VesselObjects;
public class VesselSegment
{
    [Required]
    public required int Id { get; set; }
    
    [Required]
    public required VesselPoint StartPoint { get; set; }
    
    [Required]
    public required VesselPoint EndPoint { get; set; }
    
    [Required]
    public required double Thickness { get; set; }
    
    [Required]
    public required bool IsVisible { get; set; }
}