namespace Annotations.Core.Results;

public sealed class ImageUploaderResult
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
    /// URI of created resource for return with "201 Created" response.
    /// </summary>
    public string ImageId { get; set; } = string.Empty;
}