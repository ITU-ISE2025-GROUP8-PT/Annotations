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
/*
private string? image;//byte array as string
private bool error = false;
private bool imgExist = true;
private string? altText;
*/

   
    /*

    protected override async Task OnInitializedAsync()
    {
        var httpContext = httpContextAccessor.HttpContext ?? throw new InvalidOperationException("No HttpContext available");//make connection

        var accessToken = await httpContext.GetTokenAsync("access_token") ?? throw new InvalidOperationException("No access_token was saved");//authorization

        var client = ClientFactory.CreateClient();
        client.BaseAddress = new("https://localhost:7250");//connect to Web API

        using var requestMessage = new HttpRequestMessage(HttpMethod.Get, $"/images/{imageId}");//access endpoint
        requestMessage.Headers.Authorization = new("Bearer", accessToken);//bearer token for authorization

        using var response = await client.SendAsync(requestMessage);//sending the response
        try
        {
            var jsonString = await response.Content.ReadAsStringAsync();//string of JSON file of the image as a string
            if (jsonString.Length == 0)
            {
                imgExist = false;
            }
            var imageObject = System.Text.Json.JsonSerializer.Deserialize<ImageModel>(jsonString) ?? throw new NoNullAllowedException();//JSON file becomes imageModel object
            var imageData = imageObject.ImageData;
            altText = imageObject.Description;
            image = System.Convert.ToBase64String(imageData);//byte array to string
        }
        catch 
        {
            Console.WriteLine("Status code: " + response.StatusCode);
            image = response.StatusCode.ToString();
            
            error = true;//there has been an error
        }
        services.add;
        return services; */
    
