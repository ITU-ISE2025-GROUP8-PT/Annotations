namespace Annotations.Core.Models;

public class ImageModel
{
    public required int Id { get; set; }
    public required string Title { get; set; }
    public required string Description { get; set; }
    public required byte[] ImageData { get; set; }
    public required string Category { get; set; }
    public required List<int> DatasetsIds { get; set; }

}