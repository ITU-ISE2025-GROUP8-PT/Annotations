using Annotations.Core.Entities;
using Annotations.Core.Models;
using Azure;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Azure;

namespace Annotations.API.Services;

public record ValidationResponse(bool Success, string Message);
public record ImageData(ImageModel Image, string JSONString);

public record GetImageResult(bool Success, string image);


public interface IImageService

{
    ValidationResponse ValidateImage(IFormFile file);
    BlobContainerClient createContainer();
    Task<string> convertToJSONString(BlobClient blobClient);
    Task UploadingImage(IFormFile image, int counter, string category);
    void UploadImageError(ValidationResponse response);
    Task<HashSet<string>> Filter(string category);
    Task<DatasetModel> GetDataset(string dataset);
    Task<GetImageResult> GetImage(string imageId, CancellationTokenSource cts, BlobContainerClient containerClient);
    Task<DatasetModel[]> GetAllDatasets();
    Task<bool> DeleteImage(string imageId, CancellationTokenSource cts);




}

public class ImageService: IImageService
{
    private static string[] _arrayOfFileExtension = {"png", "jpg", "jpeg"};
    private readonly IAzureClientFactory<BlobServiceClient> _clientFactory;
    private readonly AnnotationsDbContext _DbContext;

    public ImageService(IAzureClientFactory<BlobServiceClient> clientFactory , AnnotationsDbContext context)
    {
        _clientFactory = clientFactory;
        _DbContext = context;
    }
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
    public BlobContainerClient createContainer()
    {
        var blobServiceClient = _clientFactory.CreateClient("Default");
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
    
    /// <summary>
    ///Below functionality of cross-adding the image to the assigned datasets
    ///is overriden by the hard-code creation of datasets in Api, Program.cs
    ///
    /// The idea for future reference is that when uploading an image, the user
    ///inputs the names (ids) of datasets, and we can use below code (without
    ///hard-coded ids) to cross-add the images to the relevant datasets.
    /// </summary>
    /// <param name="thisImage"></param>
    private async Task AddImagesToDatasets(ImageModel thisImage)
    {
        var neededDataset = _DbContext.Datasets.Select(Dataset => Dataset)
            .Where(Dataset =>
                Dataset.Id == 1 || Dataset.Id == 2); //TODO dont do this - this is hardcoded for testing

        foreach (Dataset dataset in neededDataset)

        {
            dataset.ImageIds.Add(thisImage.Id); //adds images to the datasets
            await _DbContext.SaveChangesAsync();
        }
    }

    private async Task UploadAsJSON(int counter, string jsonString)
    {
        var containerClient = createContainer();
        var byteContent = System.Text.Encoding.UTF8.GetBytes(jsonString); //JSON string becomes byte array
        BlobClient thisImageBlobClient = containerClient.GetBlobClient($"{counter}.json");

        var blobHeaders = new BlobHttpHeaders
        {
            ContentType = "application/json"
        };

        // Trigger the upload function to push the data to blob
        await thisImageBlobClient.UploadAsync(new MemoryStream(byteContent),
            blobHeaders); //uploaded as byte array
        //Should it be uploaded as a string instead? So far all endpoints retrieve the JSON files as strings?
    }
    public async Task UploadingImage(IFormFile image, int counter, string category)
    {
            //fileExtension will always be a proper fileExtension because of the ValidateImage method
            using (MemoryStream ms = new MemoryStream())
            {
                await image.OpenReadStream().CopyToAsync(ms);
                var thisImage =
                    new ImageModel() //TODO: dont do this - the title, description and datasetsId are hardcoded
                    {
                        Id = counter,
                        Title = "idk",
                        Description = "description",
                        ImageData = ms.ToArray(),
                        Category = category,
                        DatasetsIds = new List<int>() { 1, 2 },
                    };
               
                await AddImagesToDatasets(thisImage);
                string jsonString = System.Text.Json.JsonSerializer.Serialize(thisImage); //objects becomes JSON string
                await UploadAsJSON(counter, jsonString);

            }
    }

    public void UploadImageError(ValidationResponse response)
    {
        Console.WriteLine("rejecting image");
        Console.WriteLine(response.Message);
    }

    
    public async Task<ImageData> GetImageForFiltering(BlobContainerClient containerClient, BlobItem blobItem)
    {
        var blobClient = containerClient.GetBlobClient(blobItem.Name);
        string jsonString = await convertToJSONString(blobClient);
        var imageObject = System.Text.Json.JsonSerializer.Deserialize<ImageModel>(jsonString);//deserialize so it becomes imageModel
        if (imageObject == null)
        {
            throw new Exception("img object is null");
        }

        return new(imageObject, jsonString);
    }

    public async Task<HashSet<string>> Filter(string category)
    {
        var containerClient = createContainer();

        var listOfFiles = containerClient.GetBlobsAsync().AsPages();//all data inside of blobContainer
              
        HashSet<string> collection = new HashSet<string>();
        await foreach (Page<BlobItem> blobPage in listOfFiles)
        {
            foreach (BlobItem blobItem in blobPage.Values)//every image found
            {
                //goes through all images and check for the category
                var imageData = await GetImageForFiltering(containerClient, blobItem);
                if (imageData.Image.Category == category)
                {
                    collection.Add(imageData.JSONString);
                }
            }

        }
        return collection;

    }

    public async Task<DatasetModel> GetDataset(string dataset)
    {
        var datasets = _DbContext.Datasets
            .Select(u => new DatasetModel()
            {
                Id = u.Id,
                ImageIds = u.ImageIds,
                Category = u.Category,
                AnnotatedBy= u.AnnotatedBy,
                ReviewedBy = u.ReviewedBy
            }).Where(DatasetModel => DatasetModel.Id == Int32.Parse(dataset));
        //there is only one dataset with a certain Id, so no point of taking more
        var datasetModel = await datasets.FirstOrDefaultAsync();

        if (datasetModel == null)
        {
            throw new Exception("No dataset found");
        }

        return datasetModel;
    }

    public async Task<GetImageResult> GetImage(string imageId, CancellationTokenSource cts,BlobContainerClient containerClient)
    {
        //enters images
        BlobClient blobClient = containerClient.GetBlobClient(imageId + ".json");
        if (!blobClient.Exists(cts.Token).ToString()
                .Contains("404")) //checks if the blobClient is empty/couldn't find the image of that format
        {
            
            return new GetImageResult(true, await convertToJSONString(blobClient));
        }
        return new GetImageResult(false, "");
    }

    public async Task<DatasetModel[]> GetAllDatasets()
    {
        //Datasets from DBContext are transformed to DatasetModels
        var datasets = await _DbContext.Datasets
            .Select(u => new DatasetModel()
            {
                Id = u.Id,
                ImageIds = u.ImageIds,
                Category = u.Category,
                AnnotatedBy= u.AnnotatedBy,
                ReviewedBy = u.ReviewedBy
            })
            .ToListAsync();
                
        //DatasetModel list is converted to Array for sending to Blazor front-end
        return datasets.ToArray();
    }

    public async Task<bool> DeleteImage(string imageId, CancellationTokenSource cts)
    {
        var containerClient = createContainer();
        BlobClient blobClient = containerClient.GetBlobClient(imageId + ".json");
        if (!blobClient.Exists(cts.Token).ToString().Contains("404"))
        {
            /*A snapshot is a read-only version of a blob that's taken at a point in time.
            As of right now, we do not make snapshots of blobs, but it is still possible to manually create.*/
            await blobClient.DeleteAsync(snapshotsOption: DeleteSnapshotsOption.IncludeSnapshots);
            Console.WriteLine("image deleted successfully");
            return true;
        }

        return false;
    }
    
}