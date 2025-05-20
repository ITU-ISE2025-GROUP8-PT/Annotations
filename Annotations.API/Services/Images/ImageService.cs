using System.Net;
using Annotations.Core.Entities;
using Annotations.Core.Models;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Azure;


namespace Annotations.API.Services.Images;


/// <summary>
/// Defines a service for accessing images.
/// </summary>
public interface IImageService
{
    /// <summary>
    /// Retrieves an image based on its ID. This will return a result including a file stream.
    /// </summary>
    /// <param name="imageId"></param>
    /// <returns></returns>
    Task<GetImageResult> GetImageAsync(int imageId);

    /// <summary>
    /// Deletes an image based on its ID. This will mark the image as deleted in the database and remove it from Azure Storage.
    /// </summary>
    /// <param name="imageId"></param>
    /// <returns></returns>
    Task<HttpStatusCode> DeleteImageAsync(int imageId);

    /// <summary>
    /// Retrieves the metadata of an image based on its ID. This will return a model with the image's details.
    /// </summary>
    /// <param name="imageId"></param>
    /// <returns></returns>
    Task<ImageModel?> GetMetadataAsync(int imageId);

    /// <summary>
    /// Retrieves all images within a certain category.
    /// </summary>
    /// <param name="category"></param>
    /// <returns></returns>
    Task<ICollection<ImageModel>> GetImagesByCategoryAsync(string category);
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





/// <summary>
/// Implements service for accessing images using Entity Framework Core and Azure Storage.
/// </summary>
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
    /// Retrieves an image based on its ID. This will return a result including a file stream.
    /// </summary>
    /// <param name="imageId"></param>
    /// <returns> Returns an image result with an open stream of the image. </returns>
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





    /// <summary>
    /// Retrieves the metadata of an image based on its ID. This will return a model with the image's details.
    /// </summary>
    /// <param name="imageId"></param>
    /// <returns></returns>
    public async Task<ImageModel?> GetMetadataAsync(int imageId)
    {
        var image = await _dbContext.Images
            .Where(img => img.Id == imageId && !img.IsDeleted)
            .Select(img => new ImageModel
            {
                Id = img.Id,
                Title = img.Title,
                Description = img.Description,
                Category = img.Category,
            })
            .SingleOrDefaultAsync();
        return image;
    }





    /// <summary>
    /// Retrieves all images within a certain category.
    /// </summary>
    /// <param name="category"></param>
    /// <returns></returns>
    public async Task<ICollection<ImageModel>> GetImagesByCategoryAsync(string category)
    {
        return await _dbContext.Images
            .Where(img => img.Category == category && !img.IsDeleted)
            .Select(img => new ImageModel
            {
                Id = img.Id,
                Title = img.Title,
                Description = img.Description,
                Category = img.Category,
            })
            .ToListAsync();
    }
}
