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

    [Required]
    public required byte[] ImageData { get; set; } // Not required, blob storage will contain the image data.

    /// <summary>
    /// Category of the image. This is a string that can be used to group images.
    /// </summary>
	[StringLength(100)]
	public string Category { get; set; } = string.Empty;

    /// <summary>
    /// List of dataset IDs this image belongs to.
    /// </summary>
    public List<int> DatasetsIds { get; set; } = [];
}

    /*Later add foreign keys:
    UploadedBy (referencing AdminId)
    DeletedBy (referencing AdminId)
    */
