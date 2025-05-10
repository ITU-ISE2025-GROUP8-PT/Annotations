using System.ComponentModel.DataAnnotations;

namespace Annotations.Core.Entities;

public class Dataset
{
    /// <summary>
    /// Unique identifier for the dataset.
    /// </summary>
    [Required]
    public int Id { get; set; } // Not required, as it will be set by the database.

    /// <summary>
    /// List of image IDs that belong to this dataset.
    /// </summary>
    [Required]
    public required List<int> ImageIds{ get; set; }

    /// <summary>
    /// Category of the dataset. This is a string that can be used to group datasets.
    /// </summary>
    [Required]
    public required string Category { get; set; }

    [Required]
    public required int AnnotatedBy { get; set; } // Change to be user data entity.
    [Required]
    public required int ReviewedBy { get; set; } // Change to be user data entity.
}
