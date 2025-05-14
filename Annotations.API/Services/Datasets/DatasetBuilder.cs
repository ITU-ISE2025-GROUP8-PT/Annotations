using System.Net;
using Annotations.Core.Entities;


namespace Annotations.API.Services.Datasets;


/// <summary>
/// Defines a transient service for creating a new dataset.
/// </summary>
public interface IDatasetBuilder
{
    /// <summary>
    /// Title of the dataset. This is a short description that will be displayed to the user.
    /// </summary>
    string Title { get; set; }

    /// <summary>
    /// User who created the dataset. This is used to track who added the dataset to the system.
    /// </summary>
    User? CreatedBy { get; set; }

    /// <summary>
    /// Category of the dataset. This is a string that can be used to group datasets.
    /// </summary>
    string Category { get; set; }

    /// <summary>
    /// Creates a new dataset in the database and returns the result.
    /// </summary>
    /// <returns></returns>
    Task<ModifyDatasetResult> BuildAsync();
}





/// <summary>
/// Implemention of IDatasetBuilder.
/// </summary>
public class DatasetBuilder : IDatasetBuilder
{
    public string Title { get; set; } = string.Empty;
    public User? CreatedBy { get; set; }
    public string Category { get; set; } = string.Empty;


    private readonly AnnotationsDbContext _dbContext;


    private bool buildAsyncCalled;





    /// <summary>
    /// Constructor for DatasetBuilder.
    /// </summary>
    /// <param name="dbContext"></param>
    public DatasetBuilder(AnnotationsDbContext dbContext)
    {
        _dbContext = dbContext;
    }





    /// <summary>
    /// Creates a new dataset in the database and returns the result.
    /// </summary>
    /// <returns></returns>
    /// <exception cref="InvalidOperationException"></exception>
    public async Task<ModifyDatasetResult> BuildAsync()
    {
        if (buildAsyncCalled)
        {
            throw new InvalidOperationException("BuildAsync can only be called once.");
        }
        buildAsyncCalled = true;

        var problemResult = ValidateInputProperties();
        if (problemResult != null)
        {
            return problemResult;
        }

        var dataset = await CreateInDatabaseAndReturn();

        return new ModifyDatasetResult
        {
            StatusCode = (int)HttpStatusCode.Created,
            Dataset = new Core.Models.DatasetModel
            {
                Id = dataset.Id,
                Title = dataset.Title,
                Category = dataset.Category,
                CreatedAt = dataset.CreatedAt,
                CreatedBy = new Core.Models.UserModel
                {
                    UserName = dataset.CreatedBy.UserName,
                }
            }
        };
    }





    private ModifyDatasetResult? ValidateInputProperties()
    {
        if (Title == string.Empty)
            return new ModifyDatasetResult
            {
                StatusCode = (int)HttpStatusCode.BadRequest,
                Error = "No title given"
            };

        if (Category == string.Empty)
            return new ModifyDatasetResult
            {
                StatusCode = (int)HttpStatusCode.BadRequest,
                Error = "No category assigned"
            };

        if (CreatedBy == null) throw new ArgumentNullException(
        nameof(CreatedBy),
        "Coding error - Uploading user entity is missing");

        return null;
    }





    private async Task<Dataset> CreateInDatabaseAndReturn()
    {
        if (CreatedBy == null) throw new NullReferenceException(nameof(CreatedBy));

        var newDataset = new Dataset
        {
            Title = Title,
            Category = Category,
            CreatedAt = DateTime.UtcNow,
            CreatedBy = CreatedBy
        };

        await _dbContext.AddAsync(newDataset);
        await _dbContext.SaveChangesAsync();

        return newDataset;
    }
}
