using Microsoft.AspNetCore.Authentication;

namespace Annotations.Blazor;


public class ImagesServices : IImagesServices
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IHttpClientFactory _clientFactory;

    
    public ImagesServices(IHttpContextAccessor httpContextAccessor, IHttpClientFactory clientFactory)
    {
        _httpContextAccessor = httpContextAccessor;
        _clientFactory = clientFactory;
    }

    public async Task<HttpResponseMessage> createGetResponse(string requestURI)
    {
        var httpContext = _httpContextAccessor.HttpContext ??
                          throw new InvalidOperationException("No HttpContext available"); //make connection

        var accessToken = await httpContext.GetTokenAsync("access_token") ??
                          throw new InvalidOperationException("No access_token was saved"); //authorization

        var client = _clientFactory.CreateClient();
        client.BaseAddress = new("https://localhost:7250"); //connect to Web API

        using var requestMessage = new HttpRequestMessage(HttpMethod.Get, requestURI); //access endpoint
        requestMessage.Headers.Authorization = new("Bearer", accessToken); //bearer token for authorization

        return await client.SendAsync(requestMessage); //sending the response
    }
}

    
