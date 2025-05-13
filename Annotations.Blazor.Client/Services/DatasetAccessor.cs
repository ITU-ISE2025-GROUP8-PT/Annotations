using System.Net.Http.Json;
using Annotations.Core.Models;


namespace Annotations.Blazor.Client.Services;


/// <summary>
/// Defines the contract for the dataset accessor service.
/// </summary>
public interface IDatasetAccessor
{
    Task<IEnumerable<DatasetModel>> GetDatasetOverviewAsync();

    Task<DatasetModel?> GetDatasetAsync(string datasetId);
}


/// <summary>
/// This service is used to access the dataset API.
/// </summary>
/// <param name="httpClient"></param>
internal sealed class DatasetAccessor(HttpClient httpClient) : IDatasetAccessor
{
    public async Task<IEnumerable<DatasetModel>> GetDatasetOverviewAsync()
    {
        return await httpClient.GetFromJsonAsync<IEnumerable<DatasetModel>>("api/datasets/overview") ?? [];
    }

    Task<DatasetModel?> IDatasetAccessor.GetDatasetAsync(string datasetId) =>
        httpClient.GetFromJsonAsync<DatasetModel?>($"api/datasets/get/{datasetId}");
}
