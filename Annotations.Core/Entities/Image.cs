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
    /// <para>The time and date that the image was uploaded.</para>
    /// </summary>
    [Required]
    public required DateTime TimeUploaded { get; set; }

    /// <summary>
    /// <para>User that uploaded the image.</para>
    /// </summary>
    [Required]
    public required User UploadedBy { get; set; }

    /// <summary>
    /// <para>The original filename as received at upload.</para>
    /// </summary>
    [Required]
    [StringLength(255)] // Limit with most file systems.
    public required string OriginalFilename { get; set; }

    /// <summary>
    /// Image series containing this image.
    /// </summary>
    public ICollection<ImageSeries> InImageSeries { get; set; } = [];
}
