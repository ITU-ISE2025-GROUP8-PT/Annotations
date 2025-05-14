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
    /// Title of the dataset. This is a short description that will be displayed to the user.
    /// </summary>
    [Required]
    [StringLength(100)]
    public required string Title { get; set; }

    /// <summary>
    /// Description of the dataset. This is a longer description that provides more details about the dataset.
    /// </summary>
    [StringLength(1000)]
    public string Description { get; set; } = string.Empty;

    /// <summary>
    /// The number of images in the dataset. This is used to track how many images are in the dataset.
    /// </summary>
    [Required]
    public int ImageCount { get; set; } = 0;

    /// <summary>
    /// List of image IDs that belong to this dataset.
    /// </summary>
    public List<Image> Images { get; set; } = [];

    /// <summary>
    /// List of dataset entries that belong to this dataset.
    /// </summary>
    public List<DatasetEntry> Entries { get; set; } = [];

    /// <summary>
    /// Category of the dataset. This is a string that can be used to group datasets.
    /// </summary>
    [Required]
    public required string Category { get; set; }

    /// <summary>
    /// Timestamp of when the dataset was created. This is used to track when the dataset was added to the system.
    /// </summary>
    [Required]
    public required DateTime CreatedAt { get; set; }

    /// <summary>
    /// User who created the dataset. This is used to track who added the dataset to the system.
    /// </summary>
    [Required]
    public required User CreatedBy { get; set; }

    /// <summary>
    /// User who annotated the dataset. This is used to track who is set to work on this dataset.
    /// </summary>
    public User? AnnotatedBy { get; set; }

    /// <summary>
    /// User who reviewed the dataset. This is used to track who reviewed the dataset.
    /// </summary>
    public User? ReviewedBy { get; set; }

    /// <summary>
    /// Marks this dataset as deleted. This also stops another dataset from re-using the same ID.
    /// </summary>
    public bool IsDeleted { get; set; }
}
