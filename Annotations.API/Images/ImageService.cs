using Azure.Storage.Blobs;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Azure;
using Azure.Storage.Blobs.Models;
using System.Net;

namespace Annotations.API.Images;

/// <summary>
/// Defines a scoped service for retrieving images and image related data.
/// </summary>
public interface IImageService
{
    /// <summary>
    /// Obtains download stream of full image from blob storage.
    /// </summary>
    /// <param name="imageId">URI for image to download.</param>
    Task<ImageDownloadResult> DownloadImageAsync(string imageId);

    /// <summary>
    /// Deletes image from blob storage, and soft deletes its metadata entry from the database.
    /// </summary>
    /// <param name="imageId">URI for image to delete.</param>
    /// <returns></returns>
    Task<HttpStatusCode> DeleteImageAsync(string imageId);
}



public sealed class ImageDownloadResult
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
    /// Download stream. Empty stream in case of failure.
    /// </summary>
    public Stream Stream { get; set; } = Stream.Null;

    /// <summary>
    /// Content type. "text/plain" in case of failure.
    /// </summary>
    public string ContentType { get; set; } = "text/plain";
}



public class ImageService : IImageService
{
    private readonly AnnotationsDbContext _dbContext;
    private readonly IAzureClientFactory<BlobServiceClient> _clientFactory;


    public ImageService(
        AnnotationsDbContext dbContext,
        IAzureClientFactory<BlobServiceClient> clientFactory)
    {
        _dbContext = dbContext;
        _clientFactory = clientFactory;
    }


    public async Task<ImageDownloadResult> DownloadImageAsync(string imageId)
    {
        var blob = _clientFactory.CreateClient("Default")
            .GetBlobContainerClient("medical-image")
            .GetBlobClient(imageId);

        if (!blob.Exists()) return new ImageDownloadResult
        {
            StatusCode = (int) HttpStatusCode.NotFound,
            Error = "Image not found"
        };

        var properties = await blob.GetPropertiesAsync();

        return new ImageDownloadResult
        {
            StatusCode = (int) HttpStatusCode.OK,
            Stream = await blob.OpenReadAsync(),
            ContentType = properties.Value.ContentType
        };
    }


    public async Task<HttpStatusCode> DeleteImageAsync(string imageId)
    {
        var blob = _clientFactory.CreateClient("Default")
            .GetBlobContainerClient("medical-image")
            .GetBlobClient(imageId);

        if (!blob.Exists()) return HttpStatusCode.NotFound;

        await blob.DeleteAsync(snapshotsOption: DeleteSnapshotsOption.IncludeSnapshots);

        try
        {
            var imageData = await _dbContext.Images
            .Where(data => data.ImageId == imageId)
            .SingleOrDefaultAsync();

            if (imageData == null) return HttpStatusCode.NoContent;

            imageData.IsDeleted = true;
            await _dbContext.SaveChangesAsync();
        }
        catch (Exception)
        {
            await blob.UndeleteAsync();
            throw;
        }
        
        return HttpStatusCode.NoContent;
    }
}
