using Azure.Storage.Blobs;
using Microsoft.Extensions.Azure;
using Microsoft.Identity.Client;
using Azure.Storage.Blobs.Models;
using System.Net.Mime;

namespace Annotations.API.Images;

public interface IImageUploader
{
    Task Store();
}

public class ImageUploader : IImageUploader
{
    private readonly AnnotationsDbContext _context;
    private readonly IAzureClientFactory<BlobServiceClient> _clientFactory;

    private static ISet<String> validMediaTypes = new HashSet<string>(["image/jpeg", "image/png", "image/webp"]);

    public string? ImageId { get; set; }
    public ContentType? ContentType { get; set; }
    public Stream? InputStream { get; set; }

    public ImageUploader(
        AnnotationsDbContext dbContext,
        IAzureClientFactory<BlobServiceClient> clientFactory)
    {
        _context = dbContext;
        _clientFactory = clientFactory;
    }

    public async Task Store()
    {
        if (ImageId == null)     throw new ArgumentNullException(nameof(ImageId));
        if (ContentType == null) throw new ArgumentNullException(nameof(ContentType));
        if (InputStream == null) throw new ArgumentNullException(nameof(InputStream));

        ThrowBadContent();

        var blob = _clientFactory.CreateClient("Default")
            .GetBlobContainerClient("medical-image")
            .GetBlobClient(ImageId);

        var headers = new BlobHttpHeaders
        {
            ContentType = ContentType.MediaType
        };

        await blob.SetHttpHeadersAsync(headers);

        // Should complete transactionally with a normal database update.
        await blob.UploadAsync(InputStream);
    }

    // TODO: Provide a public method to validate that input validation passes these tests.
    /// <summary>
    /// Method throws an exception if instance should not allow image to be stored.
    /// </summary>
    /// <exception cref="InvalidOperationException"></exception>
    private void ThrowBadContent()
    {
        if (!validMediaTypes.Contains(ContentType!.MediaType)) throw new InvalidOperationException("Media type not allowed");
    }
}
