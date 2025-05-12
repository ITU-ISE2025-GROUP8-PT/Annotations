using System.Net;
using Annotations.Core.Entities;
using Annotations.Core.Models;
using Microsoft.EntityFrameworkCore;


namespace Annotations.API.Services.Datasets;


/// <summary>
/// Defines a scoped service for dataset information stored in the backend API database.
/// </summary>
public interface IDatasetService
{
    /// <summary>
    /// Retrieves all images within a certain category.
    /// </summary>
    /// <param name="category"></param>
    /// <returns></returns>
    Task<ICollection<ImageModel>> GetFilteredImageSetAsync(string category);

    /// <summary>
    /// Retrieves a single dataset by ID. Includes all image entries in the dataset.
    /// </summary>
    /// <param name="datasetId"></param>
    /// <returns></returns>
    Task<DatasetModel?> GetDatasetByIdAsync(int datasetId);

    /// <summary>
    /// Retrieves all existing datasets. Will not include images.
    /// </summary>
    /// <returns></returns>
    Task<ICollection<DatasetModel>> GetDatasetOverviewAsync();

    /// <summary>
    /// Deletes a dataset by ID. Marks it as deleted in the database.
    /// </summary>
    /// <param name="datasetId"></param>
    /// <returns></returns>
    Task<HttpStatusCode> DeleteDatasetAsync(int datasetId);

    /// <summary>
    /// Sets the images for a dataset. This will replace all existing images in the dataset with the provided image IDs.
    /// </summary>
    /// <param name="datasetId"></param>
    /// <param name="imageIds"></param>
    /// <returns></returns>
    Task<ModifyDatasetResult> SetImagesAsync(int datasetId, int[] imageIds);
}





/// <summary>
/// Implementation of the IDatasetService interface.
/// </summary>
public class DatasetService : IDatasetService
{
    private readonly AnnotationsDbContext _dbContext;


    /// <summary>
    /// Constructor of the service class.
    /// </summary>
    /// <param name="context"></param>
    public DatasetService(AnnotationsDbContext context)
    {
        _dbContext = context;
    }





    /// <summary>
    /// Retrieves all images within a certain category.
    /// </summary>
    /// <param name="category"></param>
    /// <returns></returns>
    public async Task<ICollection<ImageModel>> GetFilteredImageSetAsync(string category)
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





    /// <summary>
    /// Retrieves all existing datasets. Will not include images.
    /// </summary>
    /// <returns></returns>
    public async Task<ICollection<DatasetModel>> GetDatasetOverviewAsync()
    {
        return await _dbContext.Datasets
            .Include(ds => ds.CreatedBy)
            .Select(ds => ToDatasetModel(ds))
            .ToListAsync();
    }





    /// <summary>
    /// Retrieves a single dataset by ID. Includes all image entries in the dataset.
    /// </summary>
    /// <param name="datasetId"></param>
    /// <returns></returns>
    public async Task<DatasetModel?> GetDatasetByIdAsync(int datasetId)
    {
        return await _dbContext.Datasets
            .Where(ds => ds.Id == datasetId)
            .Include(ds => ds.Entries)
            .Include(ds => ds.CreatedBy)
            .Select(ds => ToDatasetModel(ds))
            .SingleOrDefaultAsync();
    }





    /// <summary>
    /// Deletes a dataset by ID. Marks it as deleted in the database.
    /// </summary>
    /// <param name="datasetId"></param>
    /// <returns></returns>
    public async Task<HttpStatusCode> DeleteDatasetAsync(int datasetId)
    {
        var dataset = await _dbContext.Datasets
            .SingleOrDefaultAsync(ds => ds.Id == datasetId);

        if (dataset == default(Dataset)) return HttpStatusCode.NotFound;
        if (dataset.IsDeleted)           return HttpStatusCode.NotFound;

        dataset.IsDeleted = true;

        _dbContext.Update(dataset);
        await _dbContext.SaveChangesAsync();

        return HttpStatusCode.NoContent;
    }





    /// <summary>
    /// Sets the images for a dataset. This will replace all existing images in the dataset with the provided image IDs.
    /// </summary>
    /// <param name="datasetId"></param>
    /// <param name="imageIds"></param>
    /// <returns></returns>
    public async Task<ModifyDatasetResult> SetImagesAsync(int datasetId, int[] imageIds)
    {
        var dataset = await _dbContext.Datasets
            .Include(ds => ds.Entries)
            .Include(ds => ds.CreatedBy)
            .SingleOrDefaultAsync(ds => ds.Id == datasetId);



        if (dataset == default(Dataset))
        {
            return DatasetNotFoundResult();
        }
        if (dataset.IsDeleted)
        {
            return DatasetMarkedAsDeletedResult(dataset);
        }
        if (imageIds.Distinct().Count() != imageIds.Length)
        {
            return DuplicateImageIdsResult();
        }



        var images = await _dbContext.Images
            .Where(e => imageIds.Contains(e.Id) && !e.IsDeleted)
            .ToListAsync();

        if (images.Count != imageIds.Length)
        {
            return MissingImagesResult();
        }



        var newEntries = new List<DatasetEntry>();
        for (int i = 0; i < images.Count; i++)
        {
            var entry = new DatasetEntry
            {
                ImageId = images[i].Id,
                DatasetId = datasetId,
                OrderNumber = i,
            };
            newEntries.Add(entry);
        }

        dataset.Entries = newEntries;

        _dbContext.Update(dataset);

        await _dbContext.SaveChangesAsync();

        return new ModifyDatasetResult
        {
            StatusCode = (int)HttpStatusCode.Created,
            Dataset = ToDatasetModel(dataset),
        };
    }



    private static ModifyDatasetResult MissingImagesResult()
    {
        return new ModifyDatasetResult
        {
            StatusCode = (int)HttpStatusCode.NotFound,
            Error = "Some images not found."
        };
    }

    private static ModifyDatasetResult DuplicateImageIdsResult()
    {
        return new ModifyDatasetResult
        {
            StatusCode = (int)HttpStatusCode.BadRequest,
            Error = "Duplicate image IDs provided"
        };
    }

    private static ModifyDatasetResult DatasetMarkedAsDeletedResult(Dataset dataset)
    {
        return new ModifyDatasetResult
        {
            StatusCode = (int)HttpStatusCode.NotFound,
            Error = $"Dataset {dataset.Id} is marked as deleted"
        };
    }

    private static ModifyDatasetResult DatasetNotFoundResult()
    {
        return new ModifyDatasetResult
        {
            StatusCode = (int)HttpStatusCode.NotFound,
            Error = "Dataset not found"
        };
    }





    private static UserModel? ToUserModel(User? user)
    {
        if (user == null)
        {
            return null;
        }
        return new UserModel
        {
            UserName = user.UserName,
        };
    }





    private static DatasetModel ToDatasetModel(Dataset dataset)
    {
        return new DatasetModel
        {
            Id = dataset.Id,
            ImageIds = dataset.Entries
                .Select(e => e.ImageId)
                .ToList(),
            Title = dataset.Title,
            Description = dataset.Description,
            Category = dataset.Category,
            CreatedAt = dataset.CreatedAt,
            CreatedBy = ToUserModel(dataset.CreatedBy),
            AnnotatedBy = ToUserModel(dataset.AnnotatedBy),
            ReviewedBy = ToUserModel(dataset.ReviewedBy),
        };
    }
}
