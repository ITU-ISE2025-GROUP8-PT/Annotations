using Azure.Storage.Blobs;
using Microsoft.Extensions.Azure;
using Azure.Storage.Blobs.Models;
using System.Net.Mime;
using Annotations.Core.Entities;

namespace Annotations.API.Images;

public interface IImageUploader
{
    ContentType? ContentType { get; set; }
    Stream? InputStream { get; set; }
    User? Uploader { get; set; }

    /// <summary>
    /// <para>Stores the image in the application data stores.</para>
    /// <para>This task can be executed once per instance. Fields must be correctly set.
    /// An exception is thrown if instance is set up incorrectly.</para>
    /// </summary>
    /// <returns></returns>
    Task StoreAsync();
}



public class ImageUploader : IImageUploader
{
    private static readonly HashSet<string> _validMediaTypes = new(["image/jpeg", "image/png", "image/webp"]);

    private readonly AnnotationsDbContext _context;
    private readonly IAzureClientFactory<BlobServiceClient> _clientFactory;

    public ContentType? ContentType { get; set; }
    public Stream? InputStream { get; set; }
    public User? Uploader { get; set; }

    public ImageUploader(
        AnnotationsDbContext dbContext,
        IAzureClientFactory<BlobServiceClient> clientFactory)
    {
        _context = dbContext;
        _clientFactory = clientFactory;
    }

    public async Task StoreAsync()
    {
        // Validate instance
        if (ContentType == null) throw new ArgumentNullException(nameof(ContentType));
        if (InputStream == null) throw new ArgumentNullException(nameof(InputStream));
        if (Uploader == null) throw new ArgumentNullException(nameof(Uploader));
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
            ContentType = ContentType.MediaType
        };
        await blob.SetHttpHeadersAsync(headers);

        // Upload image to storage
        await blob.UploadAsync(InputStream);

        // Update database

    }

    /// <summary>
    /// Method throws an exception if instance should not allow image to be stored.
    /// </summary>
    /// <exception cref="InvalidOperationException"></exception>
    private void ThrowBadContent()
    {
        if (_validMediaTypes.Contains(ContentType!.MediaType)) throw new InvalidOperationException("Media type not allowed");
    }
    // TODO: Provide a public method to validate that input validation passes these tests.
}
