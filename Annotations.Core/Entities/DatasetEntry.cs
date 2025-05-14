namespace Annotations.Core.Entities;


/// <summary>
/// Represents an entry in a dataset.
/// </summary>
public class DatasetEntry
{
    /// <summary>
    /// Unique identifier for the dataset entry. 
    /// </summary>
    public required int DatasetId { get; set; }

    /// <summary>
    /// Unique identifier for the image associated with this dataset entry. 
    /// </summary>
    public required int ImageId { get; set; }
    
    /// <summary>
    /// Number used to order images as a list. 
    /// </summary>
    public int OrderNumber { get; set; }
}
