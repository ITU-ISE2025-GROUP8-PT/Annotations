using Annotations.Core.Entities;
using Microsoft.EntityFrameworkCore;
using System.Net;

namespace Annotations.API.Datasets;

public interface IImageSeriesService
{
    /// <summary>
    /// Retrieves an image series from the database.
    /// </summary>
    Task<GetImageSeriesResult> GetImageSeriesAsync(long imageSeriesId);
}


public sealed class GetImageSeriesResult
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


    public async Task<GetImageSeriesResult> GetImageSeriesAsync(long imageSeriesId)
    {
        var imageSeries = await _dbContext.ImageSeries
            .Where(series => series.ImageSeriesId == imageSeriesId)
            .Include(series => series.ImageEntries)
            .SingleOrDefaultAsync();

        if (imageSeries == default(ImageSeries)) return new GetImageSeriesResult
        {
            StatusCode = (int)HttpStatusCode.NotFound,
            Error = "Image series not found"
        };

        return new GetImageSeriesResult
        {
            StatusCode = (int)HttpStatusCode.OK,
            ImageSeries = imageSeries
        };
    }
}
