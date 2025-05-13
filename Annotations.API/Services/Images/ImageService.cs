using System.Net;
using Annotations.Core.Entities;
using Annotations.Core.Models;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Azure;


namespace Annotations.API.Services.Images;


/// <summary>
/// The response given after validating image.
/// </summary>
/// <param name="Success"> A boolean returning true if the image is validated. </param>
/// <param name="Message"> Message containing a success or the specific error message. </param>
public record ValidationResponse(bool Success, string Message);

/// <summary>
/// Returns the needed data for the image. 
/// </summary>
/// <param name="Image"> The image model. </param>
/// <param name="JSONString"> JSONString representing the JSON file. </param>
public record ImageData(ImageModel Image, string JSONString);



/// <summary>
/// Defines a service for accessing images.
/// </summary>
public interface IImageService
{
    Task<GetImageResult> GetImageAsync(int imageId);

    Task<HttpStatusCode> DeleteImageAsync(int imageId);
}





/// <summary>
/// The result of downloading an image.
/// </summary>
public sealed class GetImageResult
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





public class ImageService: IImageService
{
    private readonly AnnotationsDbContext _dbContext;
    private readonly IAzureClientFactory<BlobServiceClient> _clientFactory;
    private readonly BlobContainerClient _containerClient;

    

    /// <summary>
    /// Constructor of the ImageService.
    /// </summary>
    /// <param name="clientFactory"> The client for the BlobService. </param>
    /// <param name="context"> Annotations database context. </param>
    public ImageService(IAzureClientFactory<BlobServiceClient> clientFactory , AnnotationsDbContext context)
    {
        _clientFactory = clientFactory;
        _dbContext = context;
        var blobServiceClient = _clientFactory.CreateClient("Default");
        _containerClient = blobServiceClient.GetBlobContainerClient("images");
    }
    
    
    

    
    /// <summary>
    /// Attempts to retrieves an image based on id
    /// </summary>
    /// <param name="imageId"></param>
    /// <returns>Returns image as JSON string if exists, otherwise empty string is returned</returns>
    public async Task<GetImageResult> GetImageAsync(int imageId)
    {
        var cts = new CancellationTokenSource(10000);
        
        var blob = _containerClient.GetBlobClient($"{imageId}");

        if (!blob.Exists(cts.Token)) return new GetImageResult
        {
            StatusCode = (int)HttpStatusCode.NotFound,
            Error = "Image not found"
        };

        var properties = await blob.GetPropertiesAsync(cancellationToken: cts.Token);

        return new GetImageResult
        {
            StatusCode = (int)HttpStatusCode.OK,
            Stream = await blob.OpenReadAsync(cancellationToken: cts.Token),
            ContentType = properties.Value.ContentType
        };
    }
    
    
    


    /// <summary>
    /// Attempts to retrieve the image
    /// If it succeeds, then it deletes the image
    /// </summary>
    /// <param name="imageId"></param>
    /// <returns> An http status code indicating whether the image was deleted or not </returns>
    public async Task<HttpStatusCode> DeleteImageAsync(int imageId)
    {
        var imageData = await _dbContext.Images
            .SingleOrDefaultAsync(data => data.Id == imageId);

        var cts = new CancellationTokenSource(10000);
        
        BlobClient blobClient = _containerClient.GetBlobClient($"{imageId}");
        var blobDeleteResult = await blobClient.DeleteIfExistsAsync(DeleteSnapshotsOption.IncludeSnapshots, cancellationToken: cts.Token);

        if (imageData != default(Image))
        {
            imageData.IsDeleted = true;
        }

        return blobDeleteResult ? HttpStatusCode.NoContent : HttpStatusCode.NotFound;
    }
}
