using Annotations.Core.Entities;
using System.Net;

namespace Annotations.API.Datasets
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
        ICollection<string> ImageIds { get; set; } // TODO: Implement this functionality.

        /// <summary>
        /// User that created the image series.
        /// </summary>
        User? CreatedBy { get; set; }

        /// <summary>
        /// Category of the new image series.
        /// </summary>
        string Category { get; set; }

        /// <summary>
        /// <para>Creates the image series in the application data stores.</para>
        /// <para>This task can be executed once per instance. Fields must be correctly set.
        /// An exception is thrown if instance is set up incorrectly.</para>
        /// </summary>
        /// <returns></returns>
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


        public async Task<ImageSeriesBuilderResult> BuildAsync()
        {
            if (buildStarted)
            {
                throw new InvalidOperationException("Operation was already started.");
            }
            buildStarted = true;

            var problemResult = ValidateInputProperties();
            if (problemResult != null)
            {
                return problemResult;
            }

            var imageSeries = await CreateInDatabaseAndReturn();

            return new ImageSeriesBuilderResult
            {
                StatusCode = (int)HttpStatusCode.Created,
                ImageSeries = imageSeries
            };
        }


        private ImageSeriesBuilderResult? ValidateInputProperties()
        {
            if (Name == string.Empty)
                return new ImageSeriesBuilderResult
                { 
                    StatusCode = (int) HttpStatusCode.BadRequest,
                    Error = "No name given"
                };

            if (Category == string.Empty) // TODO: Should be one of a selection of categories?
                return new ImageSeriesBuilderResult
                {
                    StatusCode = (int)HttpStatusCode.BadRequest,
                    Error = "No category assigned"
                };

            if (CreatedBy == null) throw new ArgumentNullException(
            nameof(CreatedBy),
            "Coding error - Uploading user entity is missing");

            return null;
        }


        private async Task<ImageSeries> CreateInDatabaseAndReturn()
        {
            if (CreatedBy == null) throw new NullReferenceException(nameof(CreatedBy));

            var imageSeriesEntity = new ImageSeries
            {
                Name = Name,
                Category = Category,
                TimeCreated = DateTime.UtcNow,
                CreatedBy = CreatedBy
            };

            await _dbContext.AddAsync(imageSeriesEntity);
            await _dbContext.SaveChangesAsync();

            return imageSeriesEntity;
        }
    }
}
