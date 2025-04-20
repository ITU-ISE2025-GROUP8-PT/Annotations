using Azure.Storage.Blobs;
using Microsoft.Extensions.Azure;
using Azure.Storage.Blobs.Models;
using System.Net.Mime;
using Annotations.Core.Entities;
using Microsoft.IdentityModel.Tokens;

namespace Annotations.API.Images;

/// <summary>
/// Defines a transient service with which a single image can be uploaded to the Annotations application.
/// </summary>
public interface IImageUploader
{
    string? OriginalFilename { get; set; }
    string? ContentType { get; set; }
    Stream? InputStream { get; set; }
    User? UploadedBy { get; set; }

    /// <summary>
    /// <para>Stores the image in the application data stores.</para>
    /// <para>This task can be executed once per instance. Fields must be correctly set.
    /// An exception is thrown if instance is set up incorrectly.</para>
    /// </summary>
    /// <returns></returns>
    Task StoreAsync();
}



public sealed class ImageUploaderResult
{
    public required bool Success { get; set; }
    public string Error { get; set; } = String.Empty;
    public string ImageId { get; set; } = String.Empty;
}



/// <summary>
/// Implementation of IImageUploader service for Annotations using <c>AnnotationsDbContext</c> and Azure Storage.
/// </summary>
public class ImageUploader : IImageUploader
{
    private static readonly HashSet<string> _validMediaTypes = new(["image/jpeg", "image/png", "image/webp"]);

    private readonly AnnotationsDbContext _dbContext;
    private readonly IAzureClientFactory<BlobServiceClient> _clientFactory;

    public string? OriginalFilename { get; set; }
    public string? ContentType { get; set; }
    public Stream? InputStream { get; set; }
    public User? UploadedBy { get; set; }

    public ImageUploader(
        AnnotationsDbContext dbContext,
        IAzureClientFactory<BlobServiceClient> clientFactory)
    {
        _dbContext = dbContext;
        _clientFactory = clientFactory;
    }

    public async Task StoreAsync()
    {
        // Validate instance
        if (OriginalFilename == null) throw new ArgumentNullException(nameof(OriginalFilename));
        if (ContentType == null) throw new ArgumentNullException(nameof(ContentType));
        if (InputStream == null) throw new ArgumentNullException(nameof(InputStream));
        if (UploadedBy == null) throw new ArgumentNullException(nameof(UploadedBy));
        ThrowBadContent();

        // Get blob client
        var imageId = Guid.NewGuid().ToString();
        var blob = _clientFactory.CreateClient("Default")
            .GetBlobContainerClient("medical-image")
            .GetBlobClient(imageId);

        if (blob.Exists()) // Handles an extremely remote collision possibility
        {
            throw new InvalidOperationException("GUID already present in medical-image blob storage");
        }

        // Set content type in headers
        var headers = new BlobHttpHeaders
        {
            ContentType = ContentType
        };
        await blob.SetHttpHeadersAsync(headers);

        // Upload image to storage
        await blob.UploadAsync(InputStream);

        // Update database
        var imageEntity = new Image
        {
            ImageId = imageId,
            TimeUploaded = DateTime.UtcNow,
            UploadedBy = UploadedBy,
            OriginalFilename = OriginalFilename
        };

        try // Blob is deleted in case of failure to update the database. 
        {
            await _dbContext.AddAsync(imageEntity);
            await _dbContext.SaveChangesAsync();
        }
        catch (Exception)
        {
            await blob.DeleteAsync();
            throw;
        }
    }

    /// <summary>
    /// Method throws an exception if instance should not allow image to be stored.
    /// </summary>
    /// <exception cref="InvalidOperationException"></exception>
    private void ThrowBadContent()
    {
        if (!_validMediaTypes.Contains(ContentType!)) throw new InvalidOperationException("Media type not allowed");
    }
    // TODO: Provide a public method to validate that input validation passes these tests.
}
