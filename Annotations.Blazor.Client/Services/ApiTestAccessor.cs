using System.Net.Http.Json;


namespace Annotations.Blazor.Client.Services;


/// <summary>
/// Defines the contract for the testing service.
/// </summary>
public interface IApiTestAccessor
{
    /// <summary>
    /// Tests the API by calling the "tryme" endpoint.
    /// </summary>
    /// <returns></returns>
    Task<IEnumerable<string>> TryMeAsync();
}



/// <summary>
/// This service is used to test the API.
/// </summary>
public sealed class ApiTestAccessor(HttpClient httpClient) : IApiTestAccessor
{
    public async Task<IEnumerable<string>> TryMeAsync() =>
        await httpClient.GetFromJsonAsync<string[]>("api/testing/tryme") ?? ["Could not fetch content from \"api/testing/tryme\""];
}
