using Azure.Storage.Blobs;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Azure;

namespace Annotations.API.Services;

public record ValidationResponse(bool Success, string Message);


public interface IImageService

{
    ValidationResponse ValidateImage(IFormFile file);
    BlobContainerClient createContainer(IAzureClientFactory<BlobServiceClient> clientFactory);
    Task<string> convertToJSONString(BlobClient blobClient);

}

public class ImageService: IImageService
{
    private static string[] _arrayOfFileExtension = {"png", "jpg", "jpeg"};
    
    /// <summary>
    /// Helping method that validates an image based on type, size, and also checks if it even contains anything
    /// Images can be JPEG, PNG and JPG, and everything else gets rejected
    /// </summary>
    /// <param name="file">The file to be validated</param>
    /// <returns>ValidationResponse - a record that contains a boolean of whether the image is validated, and a message that describes either what went wrong, or that it was successful</returns>
    public ValidationResponse ValidateImage(IFormFile file)
    {
        
        if (file.Length > 50 * 1024 * 1024) {//50MB
            return new ValidationResponse(false, "File is too large.");
        }
        if (file.Length == 0) 
        { 
            return new ValidationResponse(false, "File doesn't exist.");
        }
        foreach (string fileExtension in _arrayOfFileExtension)
        {
            if (file.ContentType.Contains(fileExtension))
            {
                //if the image is the correct type, it will be uploaded, since it fulfills the other criterias
                //upload image to db
                return new ValidationResponse(true ,"Uploaded image successfully.");
            }
        }
        //if the code reaches this point, then the file type is none of the permitted file types, so an error is thrown
        return new ValidationResponse(false, "File is not a valid image.");
        
    }
    /// <summary>
    /// Creates a BlobContainerClient
    /// </summary>
    /// <param name="clientFactory">BlobServiceClient from our AzureClientFactory</param>
    /// <returns>BlobContainerClient</returns>
    public BlobContainerClient createContainer(IAzureClientFactory<BlobServiceClient> clientFactory)
    {
        var blobServiceClient = clientFactory.CreateClient("Default");
        return blobServiceClient.GetBlobContainerClient("images");
    }

    /// <summary>
    /// Downloads a BlobClient to a memory stream.
    /// Afterward it gets converted to a JSON file in the form of a string
    /// </summary>
    /// <param name="blobClient">BlobClient</param>
    /// <returns>JSON file in the form of a string</returns>
    public async Task<string> convertToJSONString(BlobClient blobClient)
    {
        using var memoryStream = new MemoryStream();
        await blobClient.DownloadToAsync(memoryStream);
        return System.Text.Encoding.UTF8.GetString(memoryStream.ToArray());//JSON file as string
    }
    
}