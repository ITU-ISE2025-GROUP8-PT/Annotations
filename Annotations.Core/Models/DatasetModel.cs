using System.Collections;

namespace Annotations.Core.Models;

public class DatasetModel
{
    /// <summary>
    /// Unique identifier for the dataset.
    /// </summary>
    public int Id { get; set; } = -1;

    /// <summary>
    /// Title of the dataset. This is a short description that will be displayed to the user.
    /// </summary>
    public string Title { get; set; } = string.Empty;

    /// <summary>
    /// Description of the dataset. This is a longer description that provides more details about the dataset.
    /// </summary>
    public string Description { get; set; } = string.Empty;

    /// <summary>
    /// The number of images in the dataset. This is used to track how many images are in the dataset.
    /// </summary>
    public int ImageCount { get; set; } = 0;

    /// <summary>
    /// List of image IDs that belong to this dataset.
    /// </summary>
    public List<int> ImageIds { get; set; } = [];

    /// <summary>
    /// Category of the dataset. This is a string that can be used to group datasets.
    /// </summary>
    public string Category { get; set; } = string.Empty;

    /// <summary>
    /// User who created the dataset. This is used to track who added the dataset to the system.
    /// </summary>
    public UserModel? CreatedBy { get; set; }

    /// <summary>
    /// Timestamp of when the dataset was created. This is used to track when the dataset was added to the system.
    /// </summary>
    public DateTime? CreatedAt { get; set; }

    public UserModel? AnnotatedBy { get; set; }
 
    public UserModel? ReviewedBy { get; set; }
}
