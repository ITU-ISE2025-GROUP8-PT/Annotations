using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Annotations.Core.Entities;

/// <summary>
/// Entity representing an image for research use. 
/// </summary>
public class Image
{
    /// <summary>
    /// <para>A unique and randomly generated short string.</para>
    /// <para>Should also be used for Blob Storage entries.</para>
    /// </summary>
    [Key]
	public required string ImageId { get; set; }

    /// <summary>
    /// The time and date that the image was uploaded.
    /// </summary>
    [Required]
    public required DateTime TimeUploaded { get; set; }

    /// <summary>
    /// User that uploaded the image.
    /// </summary>
    [Required]
    public required User UploadedBy { get; set; }

    /// <summary>
    /// The original filename as received at upload.
    /// </summary>
    [Required]
    [StringLength(255)] // Limit with most file systems.
    public required string OriginalFilename { get; set; }

    /// <summary>
    /// Image series containing this image.
    /// </summary>
    public ICollection<ImageSeries> InImageSeries { get; set; } = [];

    /// <summary>
    /// Marks this image as deleted.
    /// </summary>
    public bool IsDeleted { get; set; }
}
