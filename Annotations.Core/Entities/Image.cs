using System.ComponentModel.DataAnnotations;

namespace Annotations.Core.Entities;

public class Image
{
    /// <summary>
    /// Unique identifier for the image.
    /// </summary>
	[Required]
	public int Id { get; set; }

    /// <summary>
    /// Title of the image. This is a short description that will be displayed to the user.
    /// </summary>
    [StringLength(100)]
    public string Title { get; set; } = string.Empty;

    /// <summary>
    /// Description of the image. This is a longer description that provides more details about the image.
    /// </summary>
    [StringLength(1000)]
    public string Description { get; set; } = string.Empty;

    /// <summary>
    /// Category of the image. This is a string that can be used to group images.
    /// </summary>
    [Required]
	[StringLength(100)]
	public required string Category { get; set; }

    /// <summary>
    /// List of datasets that this image belongs to. This is used to group images into datasets for easier management.
    /// </summary>
    public List<Dataset> Datasets { get; set; } = [];

    /// <summary>
    /// List of dataset entries that belong to this dataset.
    /// </summary>
    public List<DatasetEntry> Entries { get; set; } = [];

    /// <summary>
    /// Timestamp of when the image was uploaded. This is used to track when the image was added to the system.
    /// </summary>
    public required DateTime CreatedAt { get; set; }

    /// <summary>
    /// User who uploaded the image. This is used to track who added the image to the system.
    /// </summary>
    public required User UploadedBy { get; set; }

    /// <summary>
    /// Marks this image as deleted. This also stops another image from re-using the same ID. 
    /// </summary>
    public bool IsDeleted { get; set; }
}
