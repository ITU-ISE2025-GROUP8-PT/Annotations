using System.Net;
using Annotations.Core.Entities;
using Annotations.Core.Models;
using Microsoft.EntityFrameworkCore;


namespace Annotations.API.Services.Datasets;


public interface IDatasetService
{
    Task<ICollection<ImageModel>> GetFilteredImageSetAsync(string category);

    Task<DatasetModel?> GetDatasetByIdAsync(int datasetId);

    Task<ICollection<DatasetModel>> GetDatasetOverviewAsync();

    Task<HttpStatusCode> DeleteDatasetAsync(int datasetId);

    Task<ModifyDatasetResult> SetImagesAsync(int datasetId, int[] imageIds);
}





public class DatasetService : IDatasetService
{
    private readonly AnnotationsDbContext _dbContext;



    public DatasetService(AnnotationsDbContext context)
    {
        _dbContext = context;
    }



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



    public async Task<ICollection<DatasetModel>> GetDatasetOverviewAsync()
    {
        return await _dbContext.Datasets
            .Include(ds => ds.CreatedBy)
            .Select(ds => ToDatasetModel(ds))
            .ToListAsync();
    }



    public async Task<DatasetModel?> GetDatasetByIdAsync(int datasetId)
    {
        return await _dbContext.Datasets
            .Where(ds => ds.Id == datasetId)
            .Include(ds => ds.Entries)
            .Include(ds => ds.CreatedBy)
            .Select(ds => ToDatasetModel(ds))
            .SingleOrDefaultAsync();
    }



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
