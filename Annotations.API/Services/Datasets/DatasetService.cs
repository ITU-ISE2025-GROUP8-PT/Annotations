using System.Net;
using Annotations.Core.Entities;
using Annotations.Core.Models;
using Microsoft.EntityFrameworkCore;


namespace Annotations.API.Services.Datasets;


public interface IDatasetService
{
    Task<ICollection<ImageModel>> Filter(string category);

    Task<DatasetModel?> GetSingleDataset(int datasetId);

    Task<ICollection<DatasetModel>> GetDatasetOverview();

    Task<ModifyDatasetResult> SetImagesAsync(int datasetId, int[] imageIds);
}





public class DatasetService : IDatasetService
{
    private readonly AnnotationsDbContext _dbContext;



    public DatasetService(AnnotationsDbContext context)
    {
        _dbContext = context;
    }



    public async Task<ICollection<ImageModel>> Filter(string category)
    {
        return await _dbContext.Images
            .Where(img => img.Category == category)
            .Select(img => new ImageModel
            {
                Id = img.Id,
                Title = img.Title,
                Description = img.Description,
                Category = img.Category,
            })
            .ToListAsync();
    }



    public async Task<ICollection<DatasetModel>> GetDatasetOverview()
    {
        return await _dbContext.Datasets
            .Include(ds => ds.CreatedBy)
            .Select(ds => ToDatasetModel(ds))
            .ToListAsync();
    }



    public async Task<DatasetModel?> GetSingleDataset(int datasetId)
    {
        return await _dbContext.Datasets
            .Where(ds => ds.Id == datasetId)
            .Include(ds => ds.Entries)
            .Include(ds => ds.CreatedBy)
            .Select(ds => ToDatasetModel(ds))
            .SingleOrDefaultAsync();
    }



    public async Task<ModifyDatasetResult> SetImagesAsync(int datasetId, int[] imageIds)
    {
        var dataset = await _dbContext.Datasets
            .Include(ds => ds.Entries)
            .Include(ds => ds.CreatedBy)
            .SingleOrDefaultAsync(ds => ds.Id == datasetId);

        if (dataset == default(Dataset)) return new ModifyDatasetResult
        {
            StatusCode = (int)HttpStatusCode.NotFound,
            Error = "Dataset not found"
        };

        if (dataset.IsDeleted) return new ModifyDatasetResult
        {
            StatusCode = (int)HttpStatusCode.NotFound,
            Error = $"Dataset {dataset.Id} is marked as deleted"
        };

        var images = await _dbContext.Images
            .Where(e => imageIds.Contains(e.Id))
            .ToListAsync();

        if (images.Count != imageIds.Length) return new ModifyDatasetResult
        {
            StatusCode = (int)HttpStatusCode.NotFound,
            Error = "Some images not found or duplicates in sequence"
        };

        var confirmed = new List<Image>();
        foreach (var image in images)
        {
            if (image.IsDeleted) return new ModifyDatasetResult
            {
                StatusCode = (int)HttpStatusCode.NotFound,
                Error = $"Image {image.Id} is marked as deleted"
            };
            confirmed.Add(image);
        }

        var newEntries = new List<DatasetEntry>();
        for (int i = 0; i < confirmed.Count; i++)
        {
            var entry = new DatasetEntry
            {
                ImageId = confirmed[i].Id,
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
            StatusCode = (int)HttpStatusCode.OK,
            Dataset = ToDatasetModel(dataset),
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
