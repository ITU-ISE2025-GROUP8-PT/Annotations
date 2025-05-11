using Annotations.Core.Entities;
using Annotations.Core.Models;
using Microsoft.EntityFrameworkCore;

namespace Annotations.API.Services.Datasets;


public interface IDatasetService
{
    Task<ICollection<ImageModel>> Filter(string category);

    Task<DatasetModel?> GetSingleDataset(int datasetId);

    Task<ICollection<DatasetModel>> GetDatasetOverview();
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
            .Select(ds => new DatasetModel
            { 
                Id = ds.Id,
                Category = ds.Category,
                AnnotatedBy = ds.AnnotatedBy,
                ReviewedBy = ds.ReviewedBy,
            })
            .ToListAsync();
    }



    public async Task<DatasetModel?> GetSingleDataset(int datasetId)
    {
        return await _dbContext.Datasets
            .Where(ds => ds.Id == datasetId)
            .Include(ds => ds.ImageIds)
            .Select(ds => new DatasetModel
            {
                Id = ds.Id,
                ImageIds = ds.ImageIds,
                Category = ds.Category,
                AnnotatedBy = ds.AnnotatedBy,
                ReviewedBy = ds.ReviewedBy,
            })
            .SingleOrDefaultAsync();
    }
}
