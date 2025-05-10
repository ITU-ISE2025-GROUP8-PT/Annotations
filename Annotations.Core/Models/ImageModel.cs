namespace Annotations.Core.Models;

public class ImageModel
{
    /// <summary>
    /// Unique identifier for the image.
    /// </summary>
    public int Id { get; set; } // Not required, as it will be set by the database.

    /// <summary>
    /// Title of the image. This is a short description that will be displayed to the user.
    /// </summary>
    public string Title { get; set; } = string.Empty;

    /// <summary>
    /// Description of the image. This is a longer description that provides more details about the image.
    /// </summary>
    public string Description { get; set; } = string.Empty;

    
    public required byte[] ImageData { get; set; } // Not required, blob storage will contain the image data.

    /// <summary>
    /// Category of the image. This is a string that can be used to group images.
    /// </summary>
    public string Category { get; set; } = string.Empty;

    /// <summary>
    /// List of dataset IDs this image belongs to.
    /// </summary>
    public List<int> DatasetsIds { get; set; } = [];

    /// <summary>
    /// Marks this image as deleted.
    /// </summary>
    public bool IsDeleted { get; set; }
}
