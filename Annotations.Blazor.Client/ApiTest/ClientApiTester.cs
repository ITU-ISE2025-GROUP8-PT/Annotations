/*
 * The following code is based on https://github.com/dotnet/blazor-samples and https://github.com/dotnet/blazor-samples/tree/main/8.0/BlazorWebAppOidcBff
 * Provided by Microsoft Corporation under the MIT license.
 */

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
