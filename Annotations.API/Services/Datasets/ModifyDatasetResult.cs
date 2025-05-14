using Annotations.Core.Models;


namespace Annotations.API.Services.Datasets;


/// <summary>
/// Result of modifying a dataset.
/// </summary>
public sealed class ModifyDatasetResult
{
    /// <summary>
    /// Status code for HTTP response.
    /// </summary>
    public required int StatusCode { get; set; }

    /// <summary>
    /// Error message if applicable.
    /// </summary>
    public string Error { get; set; } = string.Empty;

    /// <summary>
    /// Model for the dataset.
    /// </summary>
    public DatasetModel? Dataset { get; set; }
}
