using System.Net.Http.Headers;
using MatBlazor;
using Microsoft.AspNetCore.Authentication;
using Annotations.Core.Results;

namespace Annotations.Blazor.ImageServices;

public class ServerImageDownloader : IImageDownloader
{
    private readonly HttpClient _httpClient;
    private readonly IHttpContextAccessor _httpContextAccessor;
    
    public ServerImageDownloader(HttpClient httpClient, IHttpContextAccessor httpContextAccessor)
    {
        _httpClient = httpClient;
        _httpContextAccessor = httpContextAccessor;
    }
    
    public async Task<DownloadResult> DownloadImageAsync(string imageId)
    {
        var httpContext = _httpContextAccessor.HttpContext ??
                          throw new InvalidOperationException("No HttpContext available from the IHttpContextAccessor!");

        var accessToken = await httpContext.GetTokenAsync("access_token") ??
                          throw new InvalidOperationException("No access_token was saved");
            
        using var requestMessage = new HttpRequestMessage(HttpMethod.Get, $"Images/Download/{imageId}");
        requestMessage.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
        
        using var response = await _httpClient.SendAsync(requestMessage);
        
        response.EnsureSuccessStatusCode();

        return new DownloadResult
        {
            Data = await response.Content.ReadAsByteArrayAsync(),
            ContentType = response.Content.Headers.ContentType!.ToString()
        };
    }
}
