namespace Annotations.Blazor.ImageServices;

public sealed class DownloadResult
{
    public required byte[] Data { get; set; }
    public required string ContentType { get; set; }
}

public interface IImageDownloader
{
    Task<DownloadResult> DownloadImageAsync(string imageId);
}