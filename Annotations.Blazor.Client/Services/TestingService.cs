using System.Net.Http.Json;


namespace Annotations.Blazor.Client.Services;


/// <summary>
/// Defines the contract for the testing service.
/// </summary>
public interface ITestingService
{
    Task<IEnumerable<string>> TryMeAsync();
}



/// <summary>
/// This service is used to test the API.
/// </summary>
internal sealed class TestingService(HttpClient httpClient) : ITestingService
{
    public async Task<IEnumerable<string>> TryMeAsync() =>
        await httpClient.GetFromJsonAsync<string[]>("api/testing/tryme") ?? ["Could not fetch content from \"api/testing/tryme\""];
}
