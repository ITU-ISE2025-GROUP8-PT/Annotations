/*
 * The following code is based on https://github.com/dotnet/blazor-samples and https://github.com/dotnet/blazor-samples/tree/main/8.0/BlazorWebAppOidcBff
 * Provided by Microsoft Corporation under the MIT license.
 */

using Microsoft.AspNetCore.Authentication;
using Annotations.Blazor.Client.ApiTest;

namespace Annotations.Blazor;
public class ServerApiTester(HttpClient httpClient, IHttpContextAccessor httpContextAccessor) : IApiTester
{
    public async Task<IEnumerable<string>> GetTestStringsAsync()
    {
        var httpContext = httpContextAccessor.HttpContext ??
            throw new InvalidOperationException("No HttpContext available from the IHttpContextAccessor!");

        var accessToken = await httpContext.GetTokenAsync("access_token") ??
            throw new InvalidOperationException("No access_token was saved");

        using var requestMessage = new HttpRequestMessage(HttpMethod.Get, "/images/APITest");
        requestMessage.Headers.Authorization = new("Bearer", accessToken);
        using var response = await httpClient.SendAsync(requestMessage);

        response.EnsureSuccessStatusCode();

        return await response.Content.ReadFromJsonAsync<string[]>() ??
            throw new IOException("No weather forecast!");
    }
}
