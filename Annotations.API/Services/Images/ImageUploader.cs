﻿using System.Net;
using Annotations.Core.Entities;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Microsoft.Extensions.Azure;


namespace Annotations.API.Services.Images;


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
    /// Category of the image. This is a string that can be used to group images.
    /// </summary>
    string Category { get; set; }

    /// <summary>
    /// <para>Stores the image in the application data stores.</para>
    /// <para>This task can be executed once per instance. Fields must be correctly set.
    /// An exception is thrown if instance is set up incorrectly.</para>
    /// </summary>
    /// <returns></returns>
    Task<ImageUploaderResult> StoreAsync();
}





/// <summary>
/// Result of the image upload.
/// </summary>
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
    public int ImageId { get; set; } = -1;
}





/// <summary>
/// Implementation of IImageUploader service for Annotations using Entity Framework Core and Azure Storage.
/// </summary>
public class ImageUploader : IImageUploader
{
    private static readonly HashSet<string> _validMediaTypes = new(["image/jpeg", "image/png", "image/webp"]);


    public string OriginalFilename { get; set; } = string.Empty;
    public string ContentType { get; set; } = string.Empty;
    public Stream? InputStream { get; set; }
    public User? UploadedBy { get; set; }
    public string Category { get; set; } = string.Empty;


    private readonly AnnotationsDbContext _dbContext;
    private readonly IAzureClientFactory<BlobServiceClient> _clientFactory;


    private bool storeAsyncCalled;





    public ImageUploader(
        AnnotationsDbContext dbContext,
        IAzureClientFactory<BlobServiceClient> clientFactory)
    {
        _dbContext = dbContext;
        _clientFactory = clientFactory;
    }





    /// <summary>
    /// Stores the image in the application data stores.
    /// </summary>
    /// <returns></returns>
    /// <exception cref="InvalidOperationException"></exception>
    public async Task<ImageUploaderResult> StoreAsync()
    {
        if (storeAsyncCalled)
        {
            throw new InvalidOperationException("StoreAsync can only be called once.");
        }
        storeAsyncCalled = true;

        var problemResult = ValidateInputProperties();
        if (problemResult != null)
        {
            return problemResult;
        }

        var dbEntry = await AddEntryToDatabase();

        try
        {
            await UploadToStorage(dbEntry.Id);
        }
        catch (Exception)
        {
            await DeleteDbEntryOnFailure(dbEntry);
            throw;
        }

        return new ImageUploaderResult
        {
            StatusCode = (int)HttpStatusCode.Created,
            ImageId = dbEntry.Id,
        };
    }





    /// <summary>
    /// Validates that the input properties are set correctly.
    /// If any of the properties are not set correctly, a suitable error message is returned.
    /// </summary>
    /// <returns></returns>
    /// <exception cref="ArgumentNullException"></exception>
    private ImageUploaderResult? ValidateInputProperties()
    {
        if (OriginalFilename == string.Empty)
            return new ImageUploaderResult
            {
                StatusCode = (int)HttpStatusCode.BadRequest,
                Error = "Filename is missing"
            };

        if (Category == string.Empty)
            return new ImageUploaderResult
            {
                StatusCode = (int)HttpStatusCode.BadRequest,
                Error = "No category assigned"
            };

        if (!_validMediaTypes.Contains(ContentType))
            return new ImageUploaderResult
            {
                StatusCode = (int)HttpStatusCode.UnsupportedMediaType,
                Error = "Media type is not allowed. Allowed types are JPEG, PNG and WebP"
            };

        if (UploadedBy == null) throw new ArgumentNullException(
            nameof(UploadedBy),
            "Coding error - Uploading user entity is missing");

        return null;
    }





    /// <summary>
    /// Uploads the image to the Azure Blob Storage.
    /// </summary>
    /// <param name="imageId"></param>
    /// <returns></returns>
    /// <exception cref="InvalidOperationException"></exception>
    private async Task UploadToStorage(int imageId)
    {
        if (ContentType == string.Empty) throw new InvalidOperationException("Media type cannot be empty");

        var blob = _clientFactory.CreateClient("Default")
            .GetBlobContainerClient("images")
            .GetBlobClient($"{imageId}");

        if (blob.Exists())
        {
            throw new InvalidOperationException("UID already present in medical-image blob storage");
        }

        var headers = new BlobHttpHeaders
        {
            ContentType = ContentType
        };

        await blob.UploadAsync(InputStream, headers);
    }





    /// <summary>
    /// Deletes the database entry. This is done if the upload to Azure Blob Storage fails.
    /// </summary>
    /// <param name="image"></param>
    /// <returns></returns>
    private async Task DeleteDbEntryOnFailure(Image image)
    {
        _dbContext.Images.Remove(image);
        await _dbContext.SaveChangesAsync();
    }





    /// <summary>
    /// Creates a new image entry in the database.
    /// </summary>
    /// <returns></returns>
    /// <exception cref="NullReferenceException"></exception>
    private async Task<Image> AddEntryToDatabase()
    {
        if (UploadedBy == null) throw new NullReferenceException(nameof(UploadedBy));

        var imageEntity = new Image
        {
            CreatedAt = DateTime.UtcNow,
            UploadedBy = UploadedBy,
            Category = Category,
        };

        await _dbContext.AddAsync(imageEntity);
        await _dbContext.SaveChangesAsync();

        Console.WriteLine($"Image {imageEntity.Id} created");

        return imageEntity;
    }
}
