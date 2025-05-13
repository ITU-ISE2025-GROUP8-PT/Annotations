using System.Net.Http.Json;
using Annotations.Core.Models;


namespace Annotations.Blazor.Client.Services;


/// <summary>
/// Defines the contract for the dataset accessor service.
/// </summary>
public interface IDatasetAccessor
{
    Task<IList<DatasetModel>> GetDatasetOverviewAsync();

    Task<DatasetModel?> GetDatasetAsync(string datasetId);

    Task<DatasetModel?> GetImagesByCategoryAsync(string category);
}


/// <summary>
/// This service is used to access the dataset API.
/// </summary>
/// <param name="httpClient"></param>
internal sealed class DatasetAccessor(HttpClient httpClient) : IDatasetAccessor
{
    public async Task<IList<DatasetModel>> GetDatasetOverviewAsync()
    {
        return await httpClient.GetFromJsonAsync<IList<DatasetModel>>("api/datasets/overview") ?? [];
    }



    public Task<DatasetModel?> GetDatasetAsync(string datasetId) =>
        httpClient.GetFromJsonAsync<DatasetModel?>($"api/datasets/get/{datasetId}");



    public Task<DatasetModel?> GetImagesByCategoryAsync(string category) =>
        httpClient.GetFromJsonAsync<DatasetModel?>($"api/datasets/filter/{category}");
}
