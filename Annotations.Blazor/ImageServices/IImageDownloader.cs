namespace Annotations.Blazor.ImageServices;

public sealed class DownloadStream
{
    public required Stream Stream { get; set; }
    public required string ContentType { get; set; }
}

public interface IImageDownloader
{
    Task<DownloadStream> DownloadImageAsync(string imageId);
}