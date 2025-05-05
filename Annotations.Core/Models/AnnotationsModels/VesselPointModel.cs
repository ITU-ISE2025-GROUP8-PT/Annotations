namespace Annotations.Core.Models;

public class VesselPointModel
{
    public required int Id { get; set; }
    public required int X { get; set; } 
    public required int Y { get; set; }
    public required string Text { get; set; }
    public required string Type { get; set; }
}
