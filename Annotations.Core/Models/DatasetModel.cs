using System.Collections;

namespace Annotations.Core.Models;

public class DatasetModel
{
    /// <summary>
    /// Unique identifier for the dataset.
    /// </summary>
    public int Id { get; set; } // Not required, as it will be set by the database.

    /// <summary>
    /// List of image IDs that belong to this dataset.
    /// </summary>
    public required List<int> ImageIds{ get; set; }

    /// <summary>
    /// Category of the dataset. This is a string that can be used to group datasets.
    /// </summary>
    public string Category { get; set; } = string.Empty;

    public required int AnnotatedBy { get; set; } // Change to be user data entity.
 
    public required int ReviewedBy { get; set; } // Change to be user data entity.
}
