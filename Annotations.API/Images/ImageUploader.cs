using Azure.Storage.Blobs;
using Microsoft.Extensions.Azure;
using Azure.Storage.Blobs.Models;
using System.Net.Mime;
using Annotations.Core.Entities;
using Microsoft.IdentityModel.Tokens;
using System.Net;

namespace Annotations.API.Images;

/// <summary>
/// Defines a transient service with which a single image can be uploaded to the Annotations application.
/// </summary>
public interface IImageUploader
{
    /// <summary>
    /// Original filename. Will be stored as metadata. 
    /// </summary>
    string OriginalFilename { get; set; }

    /// <summary>
    /// Media type being uploaded. 
    /// </summary>
    string ContentType { get; set; }

    /// <summary>
    /// Data stream to upload from. Obtained i.e. using <c>IFormFile.OpenReadStream()</c>
    /// </summary>
    Stream? InputStream { get; set; }

    /// <summary>
    /// User uploading the file. 
    /// </summary>
    User? UploadedBy { get; set; }

    /// <summary>
    /// <para>Stores the image in the application data stores.</para>
    /// <para>This task can be executed once per instance. Fields must be correctly set.
    /// An exception is thrown if instance is set up incorrectly.</para>
    /// </summary>
    /// <returns></returns>
    Task<ImageUploaderResult> StoreAsync();
}



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



/// <summary>
/// Implementation of IImageUploader service for Annotations using <c>AnnotationsDbContext</c> and Azure Storage.
/// </summary>
public class ImageUploader : IImageUploader
{
    private static readonly HashSet<string> _validMediaTypes = new(["image/jpeg", "image/png", "image/webp"]);

    private readonly AnnotationsDbContext _dbContext;
    private readonly IAzureClientFactory<BlobServiceClient> _clientFactory;

    public string OriginalFilename { get; set; } = string.Empty;
    public string ContentType { get; set; } = string.Empty;
    public Stream? InputStream { get; set; }
    public User? UploadedBy { get; set; }


    public ImageUploader(
        AnnotationsDbContext dbContext,
        IAzureClientFactory<BlobServiceClient> clientFactory)
    {
        _dbContext = dbContext;
        _clientFactory = clientFactory;
    }


    public async Task<ImageUploaderResult> StoreAsync()
    {
        var problemResult = ValidateInputProperties();
        if (problemResult != null)
        {
            return problemResult;
        }

        var imageId = await UploadToStorage();

        try
        {
            await UpdateDatabase(imageId);
        }
        catch (Exception)
        {
            await DeleteBlobForAbort(imageId);
            throw;
        }

        return new ImageUploaderResult
        {
            StatusCode = (int)HttpStatusCode.Created,
            ImageId = imageId,
        };
    }


    private ImageUploaderResult? ValidateInputProperties()
    {
        if (OriginalFilename == string.Empty) 
            return new ImageUploaderResult
            { 
                StatusCode = (int) HttpStatusCode.BadRequest,
                Error = "Filename is missing"
            };

        if (!_validMediaTypes.Contains(ContentType))
            return new ImageUploaderResult
            {
                StatusCode = (int) HttpStatusCode.UnsupportedMediaType,
                Error = "Media type is not allowed. Allowed types are JPEG, PNG and WebP"
            };

        if (UploadedBy == null) throw new ArgumentNullException(
            nameof(UploadedBy), 
            "Coding error - Uploading user entity is missing");

        return null;
    }


    private async Task<string> UploadToStorage()
    {
        if (ContentType == string.Empty) throw new InvalidOperationException("Media type cannot be empty");

        var imageId = Guid.NewGuid().ToString();

        var blob = _clientFactory.CreateClient("Default")
            .GetBlobContainerClient("medical-image")
            .GetBlobClient(imageId);

        if (blob.Exists()) // Handles an extremely remote possibility of ID collision.
        {
            throw new InvalidOperationException("GUID already present in medical-image blob storage");
        }

        var headers = new BlobHttpHeaders
        {
            ContentType = ContentType
        };

        await blob.UploadAsync(InputStream, headers);
        return imageId;
    }


    private async Task DeleteBlobForAbort(string imageId)
    {
        var blob = _clientFactory.CreateClient("Default")
            .GetBlobContainerClient("medical-image")
            .GetBlobClient(imageId);

        if (blob.Exists())
        {
            await blob.DeleteAsync();
        }
    }


    private async Task UpdateDatabase(string imageId)
    {
        if (UploadedBy == null) throw new NullReferenceException(nameof(UploadedBy));

        var imageEntity = new Image
        {
            ImageId = imageId,
            TimeUploaded = DateTime.UtcNow,
            UploadedBy = UploadedBy,
            OriginalFilename = OriginalFilename
        };

        await _dbContext.AddAsync(imageEntity);
        await _dbContext.SaveChangesAsync();
    }
}
