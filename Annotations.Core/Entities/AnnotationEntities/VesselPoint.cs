using System.ComponentModel.DataAnnotations;
namespace Annotations.Core.VesselObjects;

public class VesselPoint
{
    [Required]
    public required int Id { get; set; }
    
    [Required]
    public required int X { get; set; } 
    
    [Required]
    public required int Y { get; set; }
    
    [Required]
    public required string Text { get; set; }
    
    [Required]
    public required string Type { get; set; }
}
