namespace Annotations.Blazor;

public interface IImagesServices
{
    Task<HttpResponseMessage> createResponse(string requestURI);
}