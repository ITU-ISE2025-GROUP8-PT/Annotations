using System.Net.Http.Json;

namespace Annotations.Blazor.Client.ApiTest
{
    internal sealed class ClientApiTester(HttpClient httpClient) : IApiTester
    {
        public async Task<IEnumerable<string>> GetTestStringsAsync() =>
        await httpClient.GetFromJsonAsync<string[]>("/images/APITest") ??
            throw new IOException("No test strings obtained from API!");
    }
}
