namespace Annotations.Core.Entities;

public class DatasetEntry
{
    public required int DatasetId { get; set; }

    public required int ImageId { get; set; }
        
    public int OrderNumber { get; set; }
}
