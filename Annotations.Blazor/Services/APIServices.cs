using System.Text;
using System.Text.Json;
using Annotations.Core.Models;
using Annotations.Core.VesselObjects;
using Microsoft.AspNetCore.Authentication;

namespace Annotations.Blazor;

public interface IAPIServices
{
    Task<HttpResponseMessage> createGetResponse(string requestURI);
    Task<HttpResponseMessage> CreatePostRequest(string requestURI, VesselAnnotationModel annotation);
}
public class APIServices : IAPIServices
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IHttpClientFactory _clientFactory;

    
    public APIServices(IHttpContextAccessor httpContextAccessor, IHttpClientFactory clientFactory)
    {
        _httpContextAccessor = httpContextAccessor;
        _clientFactory = clientFactory;
    }

    /// <summary>
    /// Gets the httpContext and thereafter pass along an accesstoken that the httpcontext has.
    /// Calls the Web API. 
    /// Response is received and passed along.
    /// </summary>
    /// <param name="requestURI"></param>
    /// <returns>Response of the Web API</returns>
    /// <exception cref="InvalidOperationException"></exception>
    public async Task<HttpResponseMessage> createGetResponse(string requestURI)
    {
        var httpContext = _httpContextAccessor.HttpContext ??
                          throw new InvalidOperationException("No HttpContext available"); //receive httpContext

        var accessToken = await httpContext.GetTokenAsync("access_token") ??
                          throw new InvalidOperationException("No access_token was saved"); //authorization

        var client = _clientFactory.CreateClient();
        client.BaseAddress = new("https://localhost:7250"); //Web API address

        using var requestMessage = new HttpRequestMessage(HttpMethod.Get, requestURI); //access endpoint
        requestMessage.Headers.Authorization = new("Bearer", accessToken); //bearer token for authorization

        return await client.SendAsync(requestMessage); //sending the response
    }
    
    public async Task<HttpResponseMessage> CreatePostRequest(string requestURI, VesselAnnotationModel Annotation)
    {
        var httpContext = _httpContextAccessor.HttpContext ??
                          throw new InvalidOperationException("No HttpContext available"); //receive httpContext

        var accessToken = await httpContext.GetTokenAsync("access_token") ??
                          throw new InvalidOperationException("No access_token was saved"); //authorization

        var client = _clientFactory.CreateClient();
        client.BaseAddress = new("https://localhost:7250"); //Web API address

        using (var requestMessage = new HttpRequestMessage(HttpMethod.Post, requestURI))
        {
            requestMessage.Content = new StringContent(JsonSerializer.Serialize(Annotation),
                Encoding.UTF8, "application/json");
            var result = await client.SendAsync(requestMessage);
            
            return result;
        }
    }
}

    
