using Annotations.Core.Entities;

namespace Annotations.API.ImageSeries
{
    /// <summary>
    /// Defines a builder for creating a new image series,
    /// and adding it to the database.
    /// </summary>
    public interface IImageSeriesBuilder
    {
        /// <summary>
        /// Name of the new image series.
        /// </summary>
        string Name { get; set; }

        /// <summary>
        /// Optionally, a collection of images initially included.
        /// </summary>
        ICollection<string> ImageIds { get; set; }

        /// <summary>
        /// User that created the image series.
        /// </summary>
        User? CreatedBy { get; set; }

        /// <summary>
        /// Category of the new image series.
        /// </summary>
        string Category { get; set; }

        Task<ImageSeriesBuilderResult> BuildAsync();
    }



    public sealed class ImageSeriesBuilderResult
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
        /// Image series entity if successfully created.
        /// </summary>
        public Core.Entities.ImageSeries? ImageSeries { get; set; }
    }



    public class ImageSeriesBuilder : IImageSeriesBuilder
    {
        public string Name { get; set; } = string.Empty;
        public ICollection<string> ImageIds { get; set; } = [];
        public User? CreatedBy { get; set; }
        public string Category { get; set; } = string.Empty;


        private readonly AnnotationsDbContext _dbContext;

        private bool buildStarted;


        public ImageSeriesBuilder(AnnotationsDbContext dbContext)
        {
            _dbContext = dbContext;
        }


        public Task<ImageSeriesBuilderResult> BuildAsync()
        {
            if (buildStarted)
            {
                throw new InvalidOperationException("Operation was already started.");
            }
            buildStarted = true;

            throw new NotImplementedException();
        }
    }
}
