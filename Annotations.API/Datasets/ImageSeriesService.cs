using Annotations.Core.Entities;
using Microsoft.EntityFrameworkCore;
using System.Net;

namespace Annotations.API.Datasets;

public interface IImageSeriesService
{
    /// <summary>
    /// Retrieves an image series from the database.
    /// </summary>
    Task<ImageSeriesResult> GetImageSeriesAsync(long imageSeriesId);

    /// <summary>
    /// Marks the image series as deleted. (soft delete)
    /// </summary>
    Task<HttpStatusCode> MarkAsDeletedAsync(long imageSeriesId);

    /// <summary>
    /// Append a list of images to an existing image series.
    /// </summary>
    /// <param name="imageSeriesId">Image series to append to.</param>
    /// <param name="imageIds">An array of one or more image IDs.</param>
    /// <returns></returns>
    Task<ImageSeriesResult> AppendImagesAsync(long imageSeriesId, string[] imageIds);
}



public sealed class ImageSeriesResult
{
    /// <summary>
    /// Status code for HTTP response.
    /// </summary>
    public required int StatusCode { get; set; }

    /// <summary>
    /// Error message if applicable.
    /// </summary>
    public string Error { get; set; } = string.Empty;

    public ImageSeries? ImageSeries { get; set; }
}



public class ImageSeriesService : IImageSeriesService
{
    private readonly AnnotationsDbContext _dbContext;


    public ImageSeriesService(AnnotationsDbContext dbContext)
    {
        _dbContext = dbContext;
    }


    public async Task<ImageSeriesResult> GetImageSeriesAsync(long imageSeriesId)
    {
        var imageSeries = await _dbContext.ImageSeries
            .Where(series => series.ImageSeriesId == imageSeriesId)
            .Include(series => series.ImageEntries)
            .SingleOrDefaultAsync();

        if (imageSeries == default(ImageSeries)) return new ImageSeriesResult
        {
            StatusCode = (int)HttpStatusCode.NotFound,
            Error = "Image series not found"
        };

        if (imageSeries.IsDeleted) return new ImageSeriesResult
        {
            StatusCode = (int)HttpStatusCode.NotFound,
            Error = $"Image series {imageSeries.ImageSeriesId} is marked as deleted"
        };

        return new ImageSeriesResult
        {
            StatusCode = (int) HttpStatusCode.OK,
            ImageSeries = imageSeries
        };
    }


    public async Task<HttpStatusCode> MarkAsDeletedAsync(long imageSeriesId)
    {
        var imageSeries = await _dbContext.ImageSeries
            .Where(series => series.ImageSeriesId == imageSeriesId)
            .SingleOrDefaultAsync();

        if (imageSeries == default(ImageSeries)) return HttpStatusCode.NotFound;

        imageSeries.IsDeleted = true;
        await _dbContext.SaveChangesAsync();

        return HttpStatusCode.NoContent;
    }


    public async Task<ImageSeriesResult> AppendImagesAsync(long imageSeriesId, string[] imageIds)
    {
        var imageSeries = await _dbContext.ImageSeries
            .Where(series => series.ImageSeriesId == imageSeriesId)
            .Include(series => series.ImageEntries)
            .SingleOrDefaultAsync();

        if (imageSeries == default(ImageSeries)) return new ImageSeriesResult
        {
            StatusCode = (int) HttpStatusCode.NotFound,
            Error = "Image series not found"
        };

        if (imageSeries.IsDeleted) return new ImageSeriesResult
        {
            StatusCode = (int) HttpStatusCode.NotFound,
            Error = $"Image series {imageSeries.ImageSeriesId} is marked as deleted"
        };

        var images = await _dbContext.Images
            .Where(e => imageIds.Contains(e.ImageId))
            .ToListAsync();

        if (imageIds.Length != images.Count) return new ImageSeriesResult
        {
            StatusCode = (int) HttpStatusCode.NotFound,
            Error = "One or more images not found"
        };

        var existingEntries = new HashSet<string>(imageSeries.ImageEntries.Select(e => e.ImageId));
        foreach (var image in images)
        {
            if (image.IsDeleted) return new ImageSeriesResult
            {
                StatusCode = (int) HttpStatusCode.NotFound,
                Error = $"Image {image.ImageId} is marked as deleted"
            };
            if (existingEntries.Contains(image.ImageId)) return new ImageSeriesResult
            {
                StatusCode = (int) HttpStatusCode.BadRequest,
                Error = $"Image {image.ImageId} already in image series. Duplicate entries not allowed."
            };
            imageSeries.ImageEntries.Add(new ImageSeriesEntry 
            { 
                ImageSeriesId = imageSeries.ImageSeriesId,
                ImageId = image.ImageId,
                OrderNumber = imageSeries.ImageEntries.Count
            });
        }

        await _dbContext.SaveChangesAsync();

        return new ImageSeriesResult
        {
            StatusCode = (int) HttpStatusCode.NoContent,
            ImageSeries = await _dbContext.ImageSeries
                .Where(series => series.ImageSeriesId == imageSeriesId)
                .Include(series => series.ImageEntries)
                .SingleOrDefaultAsync()
        };
    }
}
