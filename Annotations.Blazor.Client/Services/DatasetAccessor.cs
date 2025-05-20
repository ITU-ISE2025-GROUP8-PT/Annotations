using System.Net.Http.Json;
using Annotations.Core.Models;


namespace Annotations.Blazor.Client.Services;


/// <summary>
/// Defines the contract for the dataset accessor service.
/// </summary>
public interface IDatasetAccessor
{
    /// <summary>
    /// Retrieves an overview of all datasets. This will not include images.
    /// </summary>
    /// <returns></returns>
    Task<IList<DatasetModel>> GetDatasetOverviewAsync();

    /// <summary>
    /// Retrieves a single dataset by ID. This will include all image entries in the dataset.
    /// </summary>
    /// <param name="datasetId"></param>
    /// <returns></returns>
    Task<DatasetModel?> GetDatasetAsync(string datasetId);

    /// <summary>
    /// Retrieves all images within a certain category. This will return a list of image models.
    /// </summary>
    /// <param name="category"></param>
    /// <returns></returns>
    Task<IList<ImageModel>> GetImagesByCategoryAsync(string category);
}



/// <summary>
/// This service is used to access the dataset API.
/// </summary>
/// <param name="httpClient"></param>
public sealed class DatasetAccessor(HttpClient httpClient) : IDatasetAccessor
{
    public async Task<IList<DatasetModel>> GetDatasetOverviewAsync()
    {
        return await httpClient.GetFromJsonAsync<IList<DatasetModel>>("api/datasets/overview") ?? [];
    }



    public Task<DatasetModel?> GetDatasetAsync(string datasetId) =>
        httpClient.GetFromJsonAsync<DatasetModel?>($"api/datasets/get/{datasetId}");



    public async Task<IList<ImageModel>> GetImagesByCategoryAsync(string category)
    {
        return await httpClient.GetFromJsonAsync<IList<ImageModel>>($"api/datasets/filter/{category}") ?? [];
    }
}
